using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.webapi;
using tdb.demo.webapi.Configs;

namespace tdb.demo.webapi.Controllers.V1
{
    /// <summary>
    /// 配置
    /// </summary>
    [TdbApiVersion(1)]
    [AllowAnonymous]
    public class ConfigsController : BaseController
    {
        #region 接口

        /// <summary>
        /// 获取appsettings.json配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TdbAPILog(Level = EnmTdbLogLevel.Debug)]
        public TdbRes<AppConfig> GetAppConfig()
        {
            return TdbRes.Success(DemoConfig.App!);
        }

        #endregion
    }
}
