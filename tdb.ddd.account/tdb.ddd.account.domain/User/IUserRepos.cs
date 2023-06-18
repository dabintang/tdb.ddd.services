using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.User.Aggregate;
using tdb.ddd.account.infrastructure;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.account.domain.User
{
    /// <summary>
    /// 用户仓储接口
    /// </summary>
    public interface IUserRepos : ITdbIOCScoped, ITdbIOCIntercept
    {
        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<UserAgg?> GetUserAggAsync(long userID);

        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns></returns>
        Task<UserAgg?> GetUserAggAsync(string loginName);

        /// <summary>
        /// 获取用户拥有的角色ID
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<List<long>> GetRoleIDsAsync(long userID);

        /// <summary>
        /// 保存用户聚合信息
        /// </summary>
        /// <param name="agg">用户聚合</param>
        Task SaveAsync(UserAgg agg);

        /// <summary>
        /// 保存用户角色信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="lstRoleID">用户角色ID</param>
        /// <returns></returns>
        Task SaveUserRoleAsync(long userID, List<long> lstRoleID);
    }
}
