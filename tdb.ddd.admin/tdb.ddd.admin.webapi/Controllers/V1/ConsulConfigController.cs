using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.admin.application.contracts.V1.Interface;
using tdb.ddd.contracts;
using tdb.ddd.webapi;

namespace tdb.ddd.admin.webapi.Controllers.V1
{
    /// <summary>
    /// consul配置
    /// </summary>
    [TdbApiVersion(1)]
    public class ConsulConfigController : BaseController
    {
        /// <summary>
        /// consul配置应用
        /// </summary>
        private readonly IConsulConfigApp consulConfigApp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="consulConfigApp">consul配置应用</param>
        public ConsulConfigController(IConsulConfigApp consulConfigApp)
        {
            this.consulConfigApp = consulConfigApp;
        }

        #region 接口

        /// <summary>
        /// 还原共用配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        [AllowAnonymous]
        [TdbAuthWhiteListIP]
        public async Task<TdbRes<string>> RestoreCommonConfigAsync()
        {
            return await this.consulConfigApp.RestoreCommonConfigAsync();
        }

        /// <summary>
        /// 备份共用配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<string>> BackupCommonConfigAsync()
        {
            return await this.consulConfigApp.BackupCommonConfigAsync();
        }

        /// <summary>
        /// 还原账户服务配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        [AllowAnonymous]
        [TdbAuthWhiteListIP]
        public async Task<TdbRes<string>> RestoreAccountConfigAsync()
        {
            return await this.consulConfigApp.RestoreAccountConfigAsync();
        }

        /// <summary>
        /// 备份账户服务配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<string>> BackupAccountConfigAsync()
        {
            return await this.consulConfigApp.BackupAccountConfigAsync();
        }

        /// <summary>
        /// 还原文件服务配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<string>> RestoreFilesConfigAsync()
        {
            var reqOpe = this.CreateTdbOperateReq();
            return await this.consulConfigApp.RestoreFilesConfigAsync(reqOpe);
        }

        /// <summary>
        /// 备份文件服务配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<string>> BackupFilesConfigAsync()
        {
            return await this.consulConfigApp.BackupFilesConfigAsync();
        }

        /// <summary>
        /// 还原人际关系服务配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<string>> RestoreRelationshipsConfigAsync()
        {
            var reqOpe = this.CreateTdbOperateReq();
            return await this.consulConfigApp.RestoreRelationshipsConfigAsync(reqOpe);
        }

        /// <summary>
        /// 备份人际关系服务配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<string>> BackupRelationshipsConfigAsync()
        {
            return await this.consulConfigApp.BackupRelationshipsConfigAsync();
        }

        #endregion
    }
}
