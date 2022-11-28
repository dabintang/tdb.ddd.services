using MediatR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tdb.account.application.BusMediatR;
using tdb.account.application.contracts.V1.DTO;
using tdb.account.application.contracts.V1.Interface;
using tdb.account.domain.contracts.Const;
using tdb.account.domain.contracts.Enum;
using tdb.account.domain.contracts.User;
using tdb.account.domain.User;
using tdb.account.domain.User.Aggregate;
using tdb.account.infrastructure.Config;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.repository.sqlsugar;

namespace tdb.account.application.V1
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
        public async Task<TdbRes<UserLoginRes>> LoginAsync(UserLoginReq req)
        {
            //用户领域服务
            var userService = new UserService();
            //获取用户聚合
            var userAgg = await userService.GetAsync(req.LoginName);
            if (userAgg == null || userAgg.Password != req.Password)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.IncorrectPassword, null);
            }

            //登录
            var aggResult = await userAgg.LoginAsync(req.ClientIP);

            //广播用户登录通知
            TdbMediatR.PublishNotWait(new UserLoginNotification() { User = userAgg, ClientIP = req.ClientIP, LoginTime = DateTime.Now });

            //转为对外传输类型
            var result = DTOMapper.Map<TdbRes<UserLoginResult>, TdbRes<UserLoginRes>>(aggResult);
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
            if (userAgg == null)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.UserNotExist, null);
            }

            //登录
            var aggResult = await userAgg.LoginAsync(req.ClientIP);

            //转为对外传输类型
            var result = DTOMapper.Map<TdbRes<UserLoginResult>, TdbRes<UserLoginRes>>(aggResult);
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
            if (userAgg == null)
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
            //获取操作员用户聚合
            var operatorAgg = await userService.GetAsync(req.OperatorID);
            if (operatorAgg == null)
            {
                return new TdbRes<AddUserRes>(TdbComResMsg.UserNotExist, null);
            }

            //判断指定登录名是否已存在
            if (await userService.IsExistLoginName(param.LoginName))
            {
                return new TdbRes<AddUserRes>(AccountConfig.Msg.LoginNameExist, null);
            }

            //操作人角色
            var lstOperatorRoleID = operatorAgg.LstRoleID.Value;
            //只能赋予操作人拥有的角色[超级管理员除外]
            if (param.LstRoleID != null && lstOperatorRoleID.Contains(Cst.RoleID.SuperAdmin) == false && param.LstRoleID.Except(lstOperatorRoleID)?.Count() > 0)
            {
                return new TdbRes<AddUserRes>(TdbComResMsg.InsufficientPermissions, null);
            }

            //生成用户ID
            var userID = TdbUniqueIDHelper.CreateID(AccountConfig.App.Server.ID, AccountConfig.App.Server.Seq);
            //用户聚合
            var userAgg = new UserAgg()
            {
                ID = userID,
                LoginName = param.LoginName,
                Password = param.Password,
                GenderCode = param.GenderCode,
                Birthday = param.Birthday,
                MobilePhoneValue = new MobilePhoneValueObject() { MobilePhone = param.MobilePhone ?? "", IsMobilePhoneVerified = false },
                EmailValue = new EmailValueObject() { Email = param.Email ?? "", IsEmailVerified = false },
                StatusCode = EnmInfoStatus.Enable,
                Remark = param.Remark ?? "",
                CreateInfo = new CreateInfoValueObject() { CreatorID = req.OperatorID, CreateTime = DateTime.Now },
                UpdateInfo = new UpdateInfoValueObject() { UpdaterID = req.OperatorID, UpdateTime = DateTime.Now }
            };
            userAgg.SetName(param.Name);
            userAgg.SetNickName(param.NickName);
            var resRole = userAgg.SetLstRoleID(param.LstRoleID);
            if (resRole.Code != TdbComResMsg.Success.Code)
            {
                return new TdbRes<AddUserRes>(new TdbResMsgInfo() { Code = resRole.Code, Msg = resRole.Msg }, null);
            }

            //保存
            await userService.SaveAsync(userAgg);

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(new AddUserRes() { ID = userAgg.ID });
        }

        #endregion

        #region 私有方法

        #endregion
    }
}
