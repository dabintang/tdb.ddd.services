using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.Role.Aggregate;
using tdb.ddd.contracts;

namespace tdb.account.domain.Role
{
    /// <summary>
    /// 角色仓储接口
    /// </summary>
    public interface IRoleRepos : ITdbIOCScoped
    {
        /// <summary>
        /// 获取角色聚合
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        Task<RoleAgg> GetRoleAggAsync(int roleID);

        /// <summary>
        /// 获取角色拥有的权限ID
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        Task<List<int>> GetAuthorityIDsAsync(int roleID);

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">角色聚合</param>
        Task SaveChangedAsync(RoleAgg agg);
    }
}
