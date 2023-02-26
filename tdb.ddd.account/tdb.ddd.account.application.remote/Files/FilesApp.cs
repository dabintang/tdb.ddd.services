using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.application.contracts.Remote;
using tdb.ddd.account.application.remote.Files.DTO;
using tdb.ddd.account.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.application.remote.Files
{
    /// <summary>
    /// 文件服务应用
    /// </summary>
    public class FilesApp : IFilesApp
    {
        #region 实现接口

        /// <summary>
        /// 确认文件
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> ConfirmFileAsync(long fileID)
        {
            var url = GetFullUrl("/tdb.ddd.files/v1/Files/ConfirmFile");
            var req = new ConfirmFileReq() { ID= fileID };
            var auth = TdbIOC.GetHttpContextAccessor()?.HttpContext?.GetAuthenticationHeaderValue();

            var res = await TdbHttpClient.PostAsJsonAsync<ConfirmFileReq, TdbRes<bool>>(url, reqParam: req, authentication: auth);
            return res;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取完整的url
        /// </summary>
        /// <param name="url">url后面那一段</param>
        /// <returns></returns>
        private static string GetFullUrl(string url)
        {
            return AccountConfig.Common.FilesService.WebapiRootURL + url;
        }

        #endregion
    }
}
