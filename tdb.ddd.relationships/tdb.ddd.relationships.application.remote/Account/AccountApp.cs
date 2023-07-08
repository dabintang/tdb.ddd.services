using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.application.contracts.Remote.DTO.Account;
using tdb.ddd.relationships.application.contracts.Remote.Interface;
using tdb.ddd.relationships.infrastructure.Config;

namespace tdb.ddd.relationships.application.remote.Account
{
    /// <summary>
    /// 账户服务应用
    /// </summary>
    public class AccountApp : IAccountApp
    {
        #region 实现接口

        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="authInfo">身份认证信息</param>
        /// <returns></returns>
        public async Task<UserInfoRes?> GetUserInfoByIDAsync(long userID, AuthenticationHeaderValue? authInfo)
        {
            var url = GetFullUrl("/tdb.ddd.account/v1/User/GetUserInfo");
            var res = await TdbHttpClient.GetAsync<TdbRes<UserInfoRes>>(url, new GetUserInfoByIDReq() { UserID = userID }, authInfo);
            return res?.Data;
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
            return RelationshipsConfig.Common.AccountService.WebapiRootURL + url;
        }

        #endregion
    }
}
