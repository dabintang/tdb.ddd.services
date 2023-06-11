using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Authority.Aggregate;
using tdb.ddd.account.domain.Role.Aggregate;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Authority
{
    /// <summary>
    /// 权限领域服务
    /// </summary>
    public class AuthorityService
    {
        #region 仓储

        private IAuthorityRepos? _authorityRepos;
        /// <summary>
        /// 权限仓储
        /// </summary>
        private IAuthorityRepos AuthorityRepos
        {
            get
            {
                this._authorityRepos ??= TdbIOC.GetService<IAuthorityRepos>();
                if (this._authorityRepos is null)
                {
                    throw new TdbException("权限仓储接口未实现");
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
        public async Task<AuthorityAgg?> GetAsync(long authorityID)
        {
            return await this.AuthorityRepos.GetAuthorityAggAsync(authorityID);
        }

        /// <summary>
        /// 判断指定权限是否已存在
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        /// <returns></returns>
        public async Task<bool> IsExist(long authorityID)
        {
            var authorityAgg = await this.GetAsync(authorityID);
            return (authorityAgg is not null);
        }

        #endregion

        #region 私有方法

        #endregion
    }
}
