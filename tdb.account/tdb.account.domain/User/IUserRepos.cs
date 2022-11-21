using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.User.Aggregate;
using tdb.ddd.contracts;

namespace tdb.account.domain.User
{
    /// <summary>
    /// 用户仓储接口
    /// </summary>
    public interface IUserRepos : ITdbIOCScoped
    {
        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<UserAgg> GetUserAggAsync(long userID);

        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="password">密码（MD5）</param>
        /// <returns></returns>
        Task<UserAgg> GetUserAggAsync(string loginName, string password);

        /// <summary>
        /// 获取用户拥有的角色ID
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<List<int>> GetRoleIDsAsync(long userID);

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">用户聚合</param>
        Task SaveChangedAsync(UserAgg agg);

        /// <summary>
        /// 添加登录记录
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="clientIP">登录端IP</param>
        /// <param name="loginTime">登录时间</param>
        Task AddLoginRecordAsync(long userID, string clientIP, DateTime loginTime);
    }
}
