using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.Authority.Aggregate;
using tdb.ddd.infrastructure;

namespace tdb.account.domain.Authority
{
    /// <summary>
    /// 权限领域服务
    /// </summary>
    public class AuthorityService
    {
        #region 仓储

        private IAuthorityRepos _authorityRepos;
        /// <summary>
        /// 权限仓储
        /// </summary>
        private IAuthorityRepos AuthorityRepos
        {
            get
            {
                if (this._authorityRepos == null)
                {
                    this._authorityRepos = TdbIOC.GetService<IAuthorityRepos>();
                }

                return this._authorityRepos;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 持久化
        /// </summary>
        /// <param name="agg">权限聚合</param>
        /// <returns></returns>
        public async Task SaveAsync(AuthorityAgg agg)
        {
            await this.AuthorityRepos.SaveChangedAsync(agg);
        }

        #endregion

        #region 私有方法

        #endregion
    }
}
