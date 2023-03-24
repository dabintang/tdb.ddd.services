using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.application.contracts.Remote;
using tdb.ddd.admin.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;

namespace tdb.ddd.admin.application.remote.Account
{
    /// <summary>
    /// 账户服务应用
    /// </summary>
    public class AccountApp : IAccountApp
    {
        #region 实现接口

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> InitDataAsync()
        {
            var url = GetFullUrl("/tdb.ddd.account/v1/Tools/InitData");
            var res = await TdbHttpClient.PostAsJsonAsync<TdbRes<string>>(url, null);
            if (res is null)
            {
                return TdbRes.Fail("初始化数据账户服务数据失败");
            }

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
            return AdminConfig.Common.AccountService.WebapiRootURL + url;
        }

        #endregion
    }
}
