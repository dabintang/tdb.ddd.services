using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.Authority;
using tdb.account.domain.Authority.Aggregate;
using tdb.account.repository.DBEntity;
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;

namespace tdb.account.repository
{
    /// <summary>
    /// 权限仓储
    /// </summary>
    public class AuthorityRepos : TdbRepository<AuthorityInfo>, IAuthorityRepos
    {
        #region 实现接口

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">权限聚合</param>
        public async Task SaveChangedAsync(AuthorityAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<AuthorityAgg, AuthorityInfo>(agg);

            //保存
            await this.AsUpdateable(info).ExecuteCommandAsync();
        }

        #endregion
    }
}
