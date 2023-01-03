using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Authority.Aggregate;
using tdb.ddd.contracts;

namespace tdb.ddd.account.domain.Authority
{
    /// <summary>
    /// 权限仓储接口
    /// </summary>
    public interface IAuthorityRepos : ITdbIOCScoped
    {
        /// <summary>
        /// 获取权限聚合
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        /// <returns></returns>
        Task<AuthorityAgg> GetAuthorityAggAsync(long authorityID);

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">权限聚合</param>
        Task SaveChangedAsync(AuthorityAgg agg);
    }
}
