using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.account.application.contracts.V1.DTO;
using tdb.ddd.account.application.contracts.V1.DTO.Certificate;
using tdb.ddd.account.application.contracts.V1.Interface;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.webapi;

namespace tdb.ddd.account.webapi.Controllers.V1
{
    /// <summary>
    /// 凭证
    /// </summary>
    [TdbApiVersion(1)]
    public class CertificateController : BaseController
    {
        /// <summary>
        /// 凭证应用
        /// </summary>
        private readonly ICertificateApp certificateApp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="certificateApp">凭证应用</param>
        public CertificateController(ICertificateApp certificateApp)
        {
            this.certificateApp = certificateApp;
        }

        #region 接口

        /// <summary>
        /// 凭证登录
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<TdbRes<UserLoginRes>> Login([FromBody] CertificateLoginReq req)
        {
            //客户端IP
            req.ClientIP = this.HttpContext.GetClientIP();

            //登录
            var res = await this.certificateApp.LoginAsync(req);
            return res;
        }

        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> AddCertificate([FromBody] AddCertificateReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //添加登录
            var res = await this.certificateApp.AddCertificateAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 删除凭证
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> DeleteCertificate([FromBody] DeleteCertificateReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //添加登录
            var res = await this.certificateApp.DeleteCertificateAsync(reqOpe);
            return res;
        }

        #endregion
    }
}
