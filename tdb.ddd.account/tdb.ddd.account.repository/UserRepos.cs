using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.User;
using tdb.ddd.account.domain.User.Aggregate;
using tdb.ddd.account.repository.DBEntity;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.account.infrastructure;
using Autofac.Extras.DynamicProxy;
using tdb.ddd.account.domain.contracts.Enum;
using static tdb.ddd.contracts.TdbCst;

namespace tdb.ddd.account.repository
{
    /// <summary>
    /// 用户仓储
    /// </summary>
    [Intercept(typeof(TdbCacheInterceptor))]
    public class UserRepos : TdbRepository<UserInfo>, IUserRepos
    {
        #region 实现接口

        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(AccountCst.CacheKey.HashUserByID)]
        [TdbCacheKey(0)]
        public async Task<UserAgg?> GetUserAggAsync(long userID)
        {
            //获取用户信息
            var userInfo = await this.GetFirstAsync(m => m.ID == userID && m.IsDeleted == false);
            if (userInfo is null)
            {
                return null;
            }

            //转换为聚合结构
            var userAgg = DBMapper.Map<UserInfo, UserAgg>(userInfo);
            return userAgg;
        }

        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns></returns>
        public async Task<UserAgg?> GetUserAggAsync(string loginName)
        {
            //获取用户信息
            var userInfo = await this.GetFirstAsync(m => m.LoginName == loginName && m.IsDeleted == false);
            if (userInfo is null)
            {
                return null;
            }

            //转换为聚合结构
            var userAgg = DBMapper.Map<UserInfo, UserAgg>(userInfo);
            return userAgg;
        }

        /// <summary>
        /// 获取用户拥有的角色ID
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(AccountCst.CacheKey.HashUserRoleIDsByUserID)]
        [TdbCacheKey(0)]
        public async Task<List<long>> GetRoleIDsAsync(long userID)
        {
            var list = await this.Change<UserRoleConfig>().AsQueryable().Where(m => m.UserID == userID).Select(m => m.RoleID).ToListAsync();
            return list ?? new List<long>();
        }

        /// <summary>
        /// 保存用户聚合信息
        /// </summary>
        /// <param name="agg">用户聚合</param>
        [TdbRemoveCacheHash(AccountCst.CacheKey.HashUserByID)]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task SaveAsync(UserAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<UserAgg, UserInfo>(agg);
            //更新或保存
            await this.InsertOrUpdateAsync(info);
        }

        /// <summary>
        /// 保存用户角色信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="lstRoleID">用户角色ID</param>
        /// <returns></returns>
        [TdbRemoveCacheHash(AccountCst.CacheKey.HashUserRoleIDsByUserID)]
        [TdbCacheKey(0)]
        public async Task SaveUserRoleAsync(long userID, List<long> lstRoleID)
        {
            var lstRole = new List<UserRoleConfig>();
            foreach (var roleID in lstRoleID)
            {
                lstRole.Add(new UserRoleConfig() { UserID = userID, RoleID = roleID });
            }

            var client = this.Change<UserRoleConfig>();

            //先删除原角色
            await client.AsDeleteable().Where(m => m.UserID == userID).ExecuteCommandAsync();

            //保存新角色
            if (lstRole != null && lstRole.Count > 0)
            {
                await client.AsInsertable(lstRole).ExecuteCommandAsync();
            }
        }

        #endregion
    }
}
