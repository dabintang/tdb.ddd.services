using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.application.contracts.V1.DTO;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.account.application.contracts.V1.Interface
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
        Task<TdbRes<UserLoginRes>> LoginAsync(UserLoginDTO req);

        /// <summary>
        /// 刷新用户访问令牌
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<UserLoginRes>> RefreshAccessTokenAsync(RefreshUserAccessTokenReq req);

        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<UserInfoRes>> GetUserInfoByIDAsync(GetUserInfoByIDReq req);

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新用户ID</returns>
        Task<TdbRes<AddUserRes>> AddUserAsync(TdbOperateReq<AddUserReq> req);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> UpdateUserAsync(TdbOperateReq<UpdateUserReq> req);

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> UpdateUserPasswordAsync(TdbOperateReq<UpdateUserPasswordReq> req);
    }
}
