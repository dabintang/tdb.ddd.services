using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Role.Aggregate;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Role
{
    /// <summary>
    /// 角色领域服务
    /// </summary>
    public class RoleService
    {
        #region 仓储

        private IRoleRepos? _roleRepos;
        /// <summary>
        /// 角色仓储
        /// </summary>
        private IRoleRepos RoleRepos
        {
            get
            {
                this._roleRepos ??= TdbIOC.GetService<IRoleRepos>();
                if (this._roleRepos is null)
                {
                    throw new TdbException("角色仓储接口未实现");
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
        public async Task<RoleAgg?> GetAsync(long roleID)
        {
            return await this.RoleRepos.GetRoleAggAsync(roleID);
        }

        /// <summary>
        /// 判断指定角色是否已存在
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        public async Task<bool> IsExist(long roleID)
        {
            var roleAgg = await this.GetAsync(roleID);
            return (roleAgg is not null);
        }

        #endregion
    }
}
