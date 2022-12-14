using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.User.Aggregate;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.User
{
    /// <summary>
    /// 用户领域服务
    /// </summary>
    public class UserService
    {
        #region 仓储

        private IUserRepos _userRepos;
        /// <summary>
        /// 用户仓储
        /// </summary>
        private IUserRepos UserRepos
        {
            get
            {
                this._userRepos ??= TdbIOC.GetService<IUserRepos>();
                return this._userRepos;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public async Task<UserAgg> GetAsync(long userID)
        {
            //获取用户聚合
            var userAgg = await this.UserRepos.GetUserAggAsync(userID);
            return userAgg;
        }

        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns></returns>
        public async Task<UserAgg> GetAsync(string loginName)
        {
            //获取用户聚合
            var userAgg = await this.UserRepos.GetUserAggAsync(loginName);
            return userAgg;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        /// <param name="agg">角色聚合</param>
        /// <returns></returns>
        public async Task SaveAsync(UserAgg agg)
        {
            await this.UserRepos.SaveChangedAsync(agg);
        }

        /// <summary>
        /// 添加登录记录
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="clientIP">登录端IP</param>
        /// <param name="loginTime">登录时间</param>
        public async Task AddLoginRecordAsync(long userID, string clientIP, DateTime loginTime)
        {
            await this.UserRepos.AddLoginRecordAsync(userID, clientIP, loginTime);
        }

        /// <summary>
        /// 判断指定登录名是否已存在
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns></returns>
        public async Task<bool> IsExistLoginName(string loginName)
        {
            var userAgg = await this.GetAsync(loginName);
            return (userAgg != null);
        }

        #endregion
    }
}
