using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Authority.Aggregate
{
    /// <summary>
    /// 权限聚合
    /// </summary>
    public class AuthorityAgg : TdbAggregateRoot<long>
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

        #region 值

        /// <summary>
        /// 权限名称
        /// </summary>           
        public string Name { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>           
        public string Remark { get; set; } = "";

        #endregion

        #region 行为

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await this.AuthorityRepos.SaveAsync(this);
        }

        #endregion
    }
}
