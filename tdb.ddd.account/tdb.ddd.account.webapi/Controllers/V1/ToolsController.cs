using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.account.application.contracts;
using tdb.ddd.account.application.contracts.V1.DTO;
using tdb.ddd.account.application.contracts.V1.Interface;
using tdb.ddd.contracts;
using tdb.ddd.webapi;

namespace tdb.ddd.account.webapi.Controllers.V1
{
    /// <summary>
    /// 工具
    /// </summary>
    [TdbApiVersion(1)]
    [ApiController]
    [Route("tdb.ddd.account/v{api-version:apiVersion}/[controller]/[action]")]
    public class ToolsController : ControllerBase
    {
        /// <summary>
        /// 工具应用
        /// </summary>
        private readonly IToolsApp toolsApp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="toolsApp">工具应用</param>
        public ToolsController(IToolsApp toolsApp)
        {
            this.toolsApp = toolsApp;
        }

        #region 接口

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAuthWhiteListIP]
        public async Task<TdbRes<string>> InitData()
        {
            //更新用户密码
            var res = await this.toolsApp.InitDataAsync();
            return TdbRes.Success(res);
        }

        #endregion
    }
}
