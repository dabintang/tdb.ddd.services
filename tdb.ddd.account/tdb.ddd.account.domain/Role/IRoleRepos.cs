using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Role.Aggregate;
using tdb.ddd.account.infrastructure;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.account.domain.Role
{
    /// <summary>
    /// 角色仓储接口
    /// </summary>
    public interface IRoleRepos : ITdbIOCScoped, ITdbIOCIntercept
    {
        /// <summary>
        /// 获取角色聚合
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        Task<RoleAgg?> GetRoleAggAsync(long roleID);

        /// <summary>
        /// 获取角色拥有的权限ID
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        Task<List<long>> GetAuthorityIDsAsync(long roleID);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">角色聚合</param>
        Task SaveAsync(RoleAgg agg);

        /// <summary>
        /// 保存角色权限信息
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="lstAuthorityID">角色权限ID</param>
        /// <returns></returns>
        Task SaveRoleAuthorityAsync(long roleID, List<long> lstAuthorityID);
    }
}
