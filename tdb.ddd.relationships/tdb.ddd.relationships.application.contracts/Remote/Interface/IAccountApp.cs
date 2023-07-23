using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.Remote.DTO.Account;

namespace tdb.ddd.relationships.application.contracts.Remote.Interface
{
    /// <summary>
    /// 账户服务应用接口
    /// </summary>
    public interface IAccountApp : ITdbIOCScoped
    {
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <param name="authInfo">身份认证信息</param>
        /// <returns></returns>
        Task<UserInfoRes?> GetCurrentUserInfo(AuthenticationHeaderValue? authInfo);

        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="authInfo">身份认证信息</param>
        /// <returns></returns>
        Task<UserInfoRes?> GetUserInfoByIDAsync(long userID, AuthenticationHeaderValue? authInfo);
    }
}
