using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.account.application.BusMediatR;
using tdb.ddd.account.application.contracts.V1.DTO;
using tdb.ddd.account.application.contracts.V1.Interface;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.account.domain.Role;
using tdb.ddd.account.domain.User;
using tdb.ddd.account.domain.User.Aggregate;
using tdb.ddd.account.infrastructure;
using tdb.ddd.account.infrastructure.Config;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.account.application.V1
{
    /// <summary>
    /// 用户应用
    /// </summary>
    public class UserApp : IUserApp
    {
        #region 实现接口

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<UserLoginRes>> LoginAsync(UserLoginDTO req)
        {
            //用户领域服务
            var userService = new UserService();
            //获取用户聚合
            var userAgg = await userService.GetAsync(req.LoginName);
            if (userAgg is null || userAgg.Password != req.Password)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.IncorrectPassword, null);
            }

            //登录
            var result = await this.LoginAsync(userAgg, req.ClientIP);

            //广播用户登录通知
            TdbMediatR.Publish(new UserLoginNotification(userAgg, req.ClientIP, DateTime.Now));

            return result;
        }

        /// <summary>
        /// 刷新用户访问令牌
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<UserLoginRes>> RefreshAccessTokenAsync(RefreshUserAccessTokenReq req)
        {
            //从缓存获取刷新令牌对应的用户ID
            var userID = TdbCache.Ins.Get<long?>(req.RefreshToken);
            if (userID == null)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.ExpiredRefreshToken, null);
            }

            //用户领域服务
            var userService = new UserService();
            //获取用户聚合
            var userAgg = await userService.GetAsync(userID.Value);
            if (userAgg is null)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.UserNotExist, null);
            }

            //登录
            var result = await this.LoginAsync(userAgg, req.ClientIP);

            //清除缓存
            await TdbCache.Ins.DelAsync(req.RefreshToken);

            return result;
        }

        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<UserInfoRes>> GetUserInfoByIDAsync(GetUserInfoByIDReq req)
        {
            //用户领域服务
            var userService = new UserService();
            //获取用户聚合
            var userAgg = await userService.GetAsync(req.UserID);
            if (userAgg is null)
            {
                return new TdbRes<UserInfoRes>(AccountConfig.Msg.UserNotExist, null);
            }

            //转为DTO
            var res = DTOMapper.Map<UserAgg, UserInfoRes>(userAgg);
            return TdbRes.Success(res);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新用户ID</returns>
        public async Task<TdbRes<AddUserRes>> AddUserAsync(TdbOperateReq<AddUserReq> req)
        {
            //参数
            var param = req.Param;

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //用户领域服务
            var userService = new UserService();
            //判断指定登录名是否已存在
            if (await userService.IsExistLoginName(param.LoginName))
            {
                return new TdbRes<AddUserRes>(AccountConfig.Msg.LoginNameExist, null);
            }

            if (param.LstRoleID is not null)
            {
                //只能赋予操作人拥有的角色[超级管理员除外]
                if (req.OperatorRoleIDs.Contains(TdbCst.RoleID.SuperAdmin) == false &&
                    param.LstRoleID.Except(req.OperatorRoleIDs)?.Count() > 0)
                {
                    return new TdbRes<AddUserRes>(TdbComResMsg.InsufficientPermissions, null);
                }

                //角色领域服务
                var roleService = new RoleService();
                //判断角色ID是否存在
                foreach (var roleID in param.LstRoleID)
                {
                    if (await roleService.IsExist(roleID) == false)
                    {
                        return new TdbRes<AddUserRes>(AccountConfig.Msg.RoleNotExist.FromNewMsg($"角色不存在[{roleID}]"), null);
                    }
                }
            }

            //生成用户ID
            var userID = AccountUniqueIDHelper.CreateID();
            //用户聚合
            var userAgg = new UserAgg()
            {
                ID = userID,
                LoginName = param.LoginName,
                Password = param.Password,
                GenderCode = param.GenderCode,
                Birthday = param.Birthday,
                StatusCode = EnmInfoStatus.Enable,
                Remark = param.Remark ?? "",
                CreateInfo = new CreateInfoValueObject() { CreatorID = req.OperatorID, CreateTime = DateTime.Now },
                UpdateInfo = new UpdateInfoValueObject() { UpdaterID = req.OperatorID, UpdateTime = DateTime.Now }
            };

            userAgg.SetName(param.Name);
            userAgg.SetNickName(param.NickName);
            userAgg.SetMobilePhone(param.MobilePhone);
            userAgg.SetEmail(param.Email);
            userAgg.SetHeadImgID(param.HeadImgID);

            //设置权限
            if (param.LstRoleID is not null)
            {
                await userAgg.SetRoleAndSaveAsync(param.LstRoleID);
            }

            //保存
            await userAgg.SaveAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(new AddUserRes() { ID = userAgg.ID });
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> UpdateUserAsync(TdbOperateReq<UpdateUserReq> req)
        {
            //参数
            var param = req.Param;

            //用户领域服务
            var userService = new UserService();

            //判断权限（用户自己或拥有[用户增删改权限]者可以修改）
            if (req.OperatorID != param.ID && req.OperatorAuthorityIDs.Contains(TdbCst.AuthorityID.AccountUserManage) == false)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //获取用户聚合
            var userAgg = await userService.GetAsync(param.ID);
            if (userAgg is null)
            {
                return new TdbRes<bool>(AccountConfig.Msg.UserNotExist, false);
            }

            //更新用户信息
            userAgg.SetName(param.Name);
            userAgg.SetNickName(param.NickName);
            userAgg.GenderCode = param.GenderCode;
            userAgg.Birthday = param.Birthday;
            userAgg.SetHeadImgID(param.HeadImgID);
            userAgg.SetMobilePhone(param.MobilePhone);
            userAgg.SetEmail(param.Email);
            userAgg.Remark = param.Remark ?? "";
            userAgg.UpdateInfo.UpdaterID = req.OperatorID;
            userAgg.UpdateInfo.UpdateTime = req.OperationTime;

            //保存
            await userAgg.SaveAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> UpdateUserPasswordAsync(TdbOperateReq<UpdateUserPasswordReq> req)
        {
            //参数
            var param = req.Param;

            //用户领域服务
            var userService = new UserService();

            //判断权限（用户自己或拥有[用户增删改权限]者可以修改）
            if (req.OperatorID != param.ID && req.OperatorAuthorityIDs.Contains(TdbCst.AuthorityID.AccountUserManage) == false)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //获取用户聚合
            var userAgg = await userService.GetAsync(param.ID);
            if (userAgg is null)
            {
                return new TdbRes<bool>(AccountConfig.Msg.UserNotExist, false);
            }

            //判断原密码是否正确
            if (userAgg.Password != param.OldPassword)
            {
                return new TdbRes<bool>(AccountConfig.Msg.IncorrectOldPassword, false);
            }

            //更新用户信息
            userAgg.Password = param.NewPassword;
            userAgg.UpdateInfo.UpdaterID = req.OperatorID;
            userAgg.UpdateInfo.UpdateTime = req.OperationTime;

            //保存
            await userAgg.SaveAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userAgg">用户聚合</param>
        /// <param name="clientIP">客户端IP</param>
        /// <returns></returns>
        private async Task<TdbRes<UserLoginRes>> LoginAsync(UserAgg userAgg, string clientIP)
        {
            //用户有效性
            if (userAgg.IsDisabled)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.Disableduser, null);
            }

            //角色领域服务
            var roleService = new RoleService();

            //用户权限ID
            var lstAuthorityID = new List<long>();
            //用户角色ID
            var lstRoleID = await userAgg.GetRoleIDsAsync();
            foreach (var roleID in lstRoleID)
            {
                //获取角色聚合
                var roleAgg = await roleService.GetAsync(roleID);
                if (roleAgg is not null)
                {
                    //权限ID集合
                    var lstRoleAuthorityID = await roleAgg.GetAuthorityIDsAsync();
                    lstAuthorityID.AddRange(lstRoleAuthorityID);
                }
            }

            //响应结果
            var res = new UserLoginRes
            {
                AccessToken = CreateAccessToken(userAgg, lstRoleID, lstAuthorityID, clientIP),
                AccessTokenValidSeconds = AccountConfig.Distributed.Token.AccessTokenValidSeconds,
                RefreshToken = Guid.NewGuid().ToString("N"),
                RefreshTokenValidSeconds = AccountConfig.Distributed.Token.RefreshTokenValidSeconds
            };

            //缓存刷新令牌
            TdbCache.Ins.Set(res.RefreshToken, userAgg.ID, TimeSpan.FromSeconds(res.RefreshTokenValidSeconds));

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 生成访问令牌
        /// </summary>
        /// <param name="userAgg">用户聚合</param>
        /// <param name="lstRoleID">用户角色ID</param>
        /// <param name="lstAuthorityID">用户权限ID</param>
        /// <param name="clientIP">客户端IP</param>
        /// <returns>token</returns>
        private string CreateAccessToken(UserAgg userAgg, List<long> lstRoleID, List<long> lstAuthorityID, string clientIP)
        {
            //用户基本信息
            var lstClaim = new List<Claim>
            {
                new Claim(TdbClaimTypes.UID, userAgg.ID.ToStr()),
                new Claim(TdbClaimTypes.UName, userAgg.Name),
                //客户端IP
                new Claim(TdbClaimTypes.ClientIP, clientIP),
                //角色ID
                new Claim(TdbClaimTypes.RoleID, lstRoleID.SerializeJson()),
                //权限ID
                new Claim(TdbClaimTypes.AuthorityID, lstAuthorityID.SerializeJson())
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(lstClaim),
                Issuer = AccountConfig.Common.Token.Issuer,
                //Audience = AccConfig.Consul.Token.Audience,
                Expires = DateTime.UtcNow.AddSeconds(AccountConfig.Distributed.Token.AccessTokenValidSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AccountConfig.Common.Token.SecretKey)), SecurityAlgorithms.Aes128CbcHmacSha256)//HmacSha256Signature
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        #endregion
    }
}
