using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Role;
using tdb.ddd.account.domain.Role.Aggregate;
using tdb.ddd.account.infrastructure;
using tdb.ddd.account.repository.DBEntity;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.account.repository
{
    /// <summary>
    /// 角色仓储
    /// </summary>
    [Intercept(typeof(TdbCacheInterceptor))]
    public class RoleRepos : TdbRepository<RoleInfo>, IRoleRepos
    {
        #region 实现接口

        /// <summary>
        /// 获取角色聚合
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(AccountCst.CacheKey.HashRoleByID)]
        [TdbCacheKey(0)]
        public async Task<RoleAgg?> GetRoleAggAsync(long roleID)
        {
            //获取角色信息
            var roleInfo = await this.GetByIdAsync(roleID);
            if (roleInfo is null)
            {
                return null;
            }

            //转为聚合
            var roleAgg = DBMapper.Map<RoleInfo, RoleAgg>(roleInfo);
            return roleAgg;
        }

        /// <summary>
        /// 获取角色拥有的权限ID
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(AccountCst.CacheKey.HashRoleAuthorityIDsByRoleID)]
        [TdbCacheKey(0)]
        public async Task<List<long>> GetAuthorityIDsAsync(long roleID)
        {
            var lstAuthorityID = await this.Change<RoleAuthorityConfig>().AsQueryable().Where(m => m.RoleID == roleID).Select(m => m.AuthorityID).ToListAsync();
            return lstAuthorityID ?? new List<long>();
        }

        /// <summary>
        /// 保存角色聚合信息
        /// </summary>
        /// <param name="agg">角色聚合</param>
        [TdbRemoveCacheHash(AccountCst.CacheKey.HashRoleByID)]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task SaveAsync(RoleAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<RoleAgg, RoleInfo>(agg);

            //保存角色信息
            await this.InsertOrUpdateAsync(info);
        }

        /// <summary>
        /// 保存角色权限信息
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="lstAuthorityID">角色权限ID</param>
        /// <returns></returns>
        [TdbRemoveCacheHash(AccountCst.CacheKey.HashRoleAuthorityIDsByRoleID)]
        [TdbCacheKey(0)]
        public async Task SaveRoleAuthorityAsync(long roleID, List<long> lstAuthorityID)
        {
            var lstAuthority = new List<RoleAuthorityConfig>();
            foreach (var authorityID in lstAuthorityID)
            {
                lstAuthority.Add(new RoleAuthorityConfig() { RoleID = roleID, AuthorityID = authorityID });
            }

            var client = this.Change<RoleAuthorityConfig>();

            //先删除原权限
            await client.AsDeleteable().Where(m => m.RoleID == roleID).ExecuteCommandAsync();

            //保存新权限
            if (lstAuthority != null && lstAuthority.Count > 0)
            {
                await client.AsInsertable(lstAuthority).ExecuteCommandAsync();
            }
        }

        #endregion
    }
}
