using MediatR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tdb.account.application.BusMediatR;
using tdb.account.application.contracts.V1.DTO;
using tdb.account.application.contracts.V1.Interface;
using tdb.account.domain.BusMediatR;
using tdb.account.domain.contracts.Enum;
using tdb.account.domain.contracts.User;
using tdb.account.domain.Role;
using tdb.account.domain.User;
using tdb.account.domain.User.Aggregate;
using tdb.account.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services.Bus;

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
        public async Task<TdbRes<UserLoginRes>> Login(UserLoginReq req)
        {
            //用户领域服务
            var userService = new UserService();
            //获取用户聚合
            var userAgg = await userService.GetAsync(req.LoginName, req.Password);
            if (userAgg == null)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.IncorrectPassword, null);
            }

            //登录
            var aggResult = await userAgg.LoginAsync(req.ClientIP);

            //中介者
            var mediator = TdbIOC.GetService<IMediator>();
            //广播用户登录通知
            mediator.PublishNotWait(new UserLoginNotification() { User = userAgg, ClientIP = req.ClientIP, LoginTime = DateTime.Now });

            //转为对外传输类型
            var result = DTOMapper.Map<TdbRes<UserLoginResult>, TdbRes<UserLoginRes>>(aggResult);
            return result;
        }

        /// <summary>
        /// 刷新用户访问令牌
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<UserLoginRes>> RefreshAccessToken(RefreshUserAccessTokenReq req)
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
        public async Task<TdbRes<UserInfoRes>> GetUserInfoByID(GetUserInfoByIDReq req)
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

        #endregion

        #region 私有方法

        #endregion
    }
}
