using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Authority.Aggregate;
using tdb.ddd.account.domain.Role.Aggregate;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Authority
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
        /// 根据权限ID获取权限聚合
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        /// <returns></returns>
        public async Task<AuthorityAgg> GetAsync(long authorityID)
        {
            return await this.AuthorityRepos.GetAuthorityAggAsync(authorityID);
        }

        /// <summary>
        /// 持久化
        /// </summary>
        /// <param name="agg">权限聚合</param>
        /// <returns></returns>
        public async Task SaveAsync(AuthorityAgg agg)
        {
            await this.AuthorityRepos.SaveChangedAsync(agg);
        }

        /// <summary>
        /// 判断指定权限是否已存在
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        /// <returns></returns>
        public async Task<bool> IsExist(long authorityID)
        {
            var authorityAgg = await this.GetAsync(authorityID);
            return (authorityAgg != null);
        }

        #endregion

        #region 私有方法

        #endregion
    }
}
