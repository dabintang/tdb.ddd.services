using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Role;
using tdb.ddd.account.domain.Role.Aggregate;
using tdb.ddd.account.repository.DBEntity;
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.account.repository
{
    /// <summary>
    /// 角色仓储
    /// </summary>
    public class RoleRepos : TdbRepository<RoleInfo>, IRoleRepos
    {
        #region 实现接口

        /// <summary>
        /// 获取角色聚合
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        public async Task<RoleAgg> GetRoleAggAsync(long roleID)
        {
            //获取角色信息
            var roleInfo = await TdbCache.Ins.CacheShellAsync(this.CacheKeyRoleInfo(roleID), TimeSpan.FromDays(1), async () => await this.GetByIdAsync(roleID));

            //转为聚合
            var roleAgg = DBMapper.Map<RoleInfo, RoleAgg>(roleInfo);

            return roleAgg;
        }

        /// <summary>
        /// 获取角色拥有的权限ID
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        public async Task<List<long>> GetAuthorityIDsAsync(long roleID)
        {
            return await TdbCache.Ins.CacheShellAsync(this.CacheKeyRoleAuthorityID(roleID), TimeSpan.FromDays(1), async () =>
            {
                return await this.Change<RoleAuthorityConfig>().AsQueryable().Where(m => m.RoleID == roleID).Select(m => m.AuthorityID).ToListAsync();
            });
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">角色聚合</param>
        public async Task SaveChangedAsync(RoleAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<RoleAgg, RoleInfo>(agg);

            //保存角色信息
            await this.InsertOrUpdateAsync(info);
            //移除缓存
            TdbCache.Ins.Del(this.CacheKeyRoleInfo(info.ID));

            #region 保存角色权限信息

            //懒加载属性未加载，认为没改动过
            if (agg.LstAuthorityID.IsLoaded)
            {
                //转换为数据库实体
                var lstAuthority = this.ToRoleAuthorityConfig(agg);
                //保存角色权限信息
                await this.SaveRoleAuthorityConfigAsync(agg.ID, lstAuthority);
            }

            #endregion
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 保存角色权限信息
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="lstAuthority">角色权限</param>
        /// <returns></returns>
        private async Task SaveRoleAuthorityConfigAsync(long roleID, List<RoleAuthorityConfig> lstAuthority)
        {
            var client = this.Change<RoleAuthorityConfig>();

            //先删除原权限
            await client.AsDeleteable().Where(m => m.RoleID == roleID).ExecuteCommandAsync();
            //移除缓存
            TdbCache.Ins.Del(this.CacheKeyRoleAuthorityID(roleID));

            //保存新权限
            if (lstAuthority != null && lstAuthority.Count >0)
            {
                await client.AsInsertable(lstAuthority).ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 转换为数据库实体
        /// </summary>
        /// <param name="agg">聚合</param>
        /// <returns></returns>
        private List<RoleAuthorityConfig> ToRoleAuthorityConfig(RoleAgg agg)
        {
            var list = new List<RoleAuthorityConfig>();
            foreach (var authorityID in agg.LstAuthorityID.Value)
            {
                list.Add(new RoleAuthorityConfig() { RoleID = agg.ID, AuthorityID = authorityID });
            }
            return list;
        }

        /// <summary>
        /// 获取【角色信息】缓存key
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        private string CacheKeyRoleInfo(long roleID)
        {
            return $"ReposRoleInfo_{roleID}";
        }

        /// <summary>
        /// 获取【角色权限ID】缓存key
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        private string CacheKeyRoleAuthorityID(long roleID)
        {
            return $"ReposRoleAuthorityID_{roleID}";
        }

        #endregion
    }
}
