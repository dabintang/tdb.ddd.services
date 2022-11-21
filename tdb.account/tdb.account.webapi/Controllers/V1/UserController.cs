using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using tdb.account.application.contracts.V1.DTO;
using tdb.account.application.contracts.V1.Interface;
using tdb.account.repository;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.webapi;

namespace tdb.account.webapi.Controllers.V1
{
    /// <summary>
    /// 用户
    /// </summary>
    [TdbApiVersion(1)]
    public class UserController : BaseController
    {
        /// <summary>
        /// 用户应用
        /// </summary>
        private readonly IUserApp userApp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userApp">用户应用</param>
        public UserController(IUserApp userApp)
        {
            this.userApp = userApp;
        }

        #region 接口

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [TdbAPILog]
        public async Task<TdbRes<UserLoginRes>> Login([FromBody] UserLoginReq req)
        {
            //客户端IP
            req.ClientIP = this.HttpContext.GetClientIP();

            //登录
            var res = await this.userApp.Login(req);
            return res;
        }

        /// <summary>
        /// 刷新用户访问令牌
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<TdbRes<UserLoginRes>> RefreshAccessToken([FromBody] RefreshUserAccessTokenReq req)
        {
            //客户端IP
            req.ClientIP = this.HttpContext.GetClientIP();

            //刷新用户访问令牌
            var res = await this.userApp.RefreshAccessToken(req);
            return res;
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<TdbRes<UserInfoRes>> GetCurrentUserInfo()
        {
            //参数
            var req = new GetUserInfoByIDReq()
            {
                 UserID = this.CurUser.ID
            };

            var res = await this.userApp.GetUserInfoByID(req);
            return res;
        }

        #endregion
    }
}
