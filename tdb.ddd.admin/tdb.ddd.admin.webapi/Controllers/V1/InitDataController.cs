using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.admin.application.contracts.V1.DTO;
using tdb.ddd.admin.application.contracts.V1.Interface;
using tdb.ddd.contracts;
using tdb.ddd.webapi;

namespace tdb.ddd.admin.webapi.Controllers.V1
{
    /// <summary>
    /// 初始化数据
    /// </summary>
    [TdbApiVersion(1)]
    public class InitDataController : BaseController
    {
        /// <summary>
        /// 初始化数据应用
        /// </summary>
        private readonly IInitDataApp initDataApp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initDataApp">consul配置应用</param>
        public InitDataController(IInitDataApp initDataApp)
        {
            this.initDataApp = initDataApp;
        }

        #region 接口

        /// <summary>
        /// 初始化账户服务数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        [AllowAnonymous]
        [TdbAuthWhiteListIP]
        public async Task<TdbRes<string>> InitAccountData()
        {
            return await this.initDataApp.InitAccountDataAsync();
        }

        #endregion
    }
}
