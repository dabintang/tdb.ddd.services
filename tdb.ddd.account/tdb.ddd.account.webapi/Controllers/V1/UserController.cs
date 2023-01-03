using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using tdb.ddd.account.application.contracts.V1.DTO;
using tdb.ddd.account.application.contracts.V1.Interface;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.webapi;

namespace tdb.ddd.account.webapi.Controllers.V1
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
        public async Task<TdbRes<UserLoginRes>> Login([FromBody] UserLoginDTO req)
        {
            //客户端IP
            req.ClientIP = this.HttpContext.GetClientIP();

            //登录
            var res = await this.userApp.LoginAsync(req);
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
            var res = await this.userApp.RefreshAccessTokenAsync(req);
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

            var res = await this.userApp.GetUserInfoByIDAsync(req);
            return res;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="req">参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TdbRes<UserInfoRes>> GetUserInfo([FromQuery]GetUserInfoByIDReq req)
        {
            var res = await this.userApp.GetUserInfoByIDAsync(req);
            return res;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<AddUserRes>> AddUser([FromBody] AddUserReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //添加用户
            var res = await this.userApp.AddUserAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> UpdateUser([FromBody] UpdateUserReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //更新用户
            var res = await this.userApp.UpdateUserAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> UpdateUserPassword([FromBody] UpdateUserPasswordReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //更新用户密码
            var res = await this.userApp.UpdateUserPasswordAsync(reqOpe);
            return res;
        }

        #endregion
    }
}
