using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.application.contracts.V1.DTO;
using tdb.ddd.contracts;

namespace tdb.account.application.contracts.V1.Interface
{
    /// <summary>
    /// 用户应用接口
    /// </summary>
    public interface IUserApp : ITdbIOCScoped
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<UserLoginRes>> Login(UserLoginReq req);

        /// <summary>
        /// 刷新用户访问令牌
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<UserLoginRes>> RefreshAccessToken(RefreshUserAccessTokenReq req);

        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<UserInfoRes>> GetUserInfoByID(GetUserInfoByIDReq req);

    }
}
