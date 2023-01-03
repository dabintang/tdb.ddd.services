using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Role.Aggregate;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Role
{
    /// <summary>
    /// 角色领域服务
    /// </summary>
    public class RoleService
    {
        #region 仓储

        private IRoleRepos _roleRepos;
        /// <summary>
        /// 角色仓储
        /// </summary>
        private IRoleRepos RoleRepos
        {
            get
            {
                if (this._roleRepos == null)
                {
                    this._roleRepos = TdbIOC.GetService<IRoleRepos>();
                }

                return this._roleRepos;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 根据角色ID获取角色聚合
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        public async Task<RoleAgg> GetAsync(long roleID)
        {
            return await this.RoleRepos.GetRoleAggAsync(roleID);
        }

        /// <summary>
        /// 持久化
        /// </summary>
        /// <param name="agg">角色聚合</param>
        /// <returns></returns>
        public async Task SaveAsync(RoleAgg agg)
        {
            await this.RoleRepos.SaveChangedAsync(agg);
        }

        /// <summary>
        /// 判断指定角色是否已存在
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        public async Task<bool> IsExist(long roleID)
        {
            var roleAgg = await this.GetAsync(roleID);
            return (roleAgg != null);
        }

        #endregion
    }
}
