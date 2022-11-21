using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.Authority.Aggregate;
using tdb.ddd.contracts;

namespace tdb.account.domain.Authority
{
    /// <summary>
    /// 权限仓储接口
    /// </summary>
    public interface IAuthorityRepos : ITdbIOCScoped
    {
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">权限聚合</param>
        Task SaveChangedAsync(AuthorityAgg agg);
    }
}
