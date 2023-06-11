using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Authority;
using tdb.ddd.account.domain.Authority.Aggregate;
using tdb.ddd.account.infrastructure;
using tdb.ddd.account.repository.DBEntity;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.account.repository
{
    /// <summary>
    /// 权限仓储
    /// </summary>
    [Intercept(typeof(TdbCacheInterceptor))]
    public class AuthorityRepos : TdbRepository<AuthorityInfo>, IAuthorityRepos
    {
        #region 实现接口

        /// <summary>
        /// 获取权限聚合
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(AccountCst.CacheKey.HashAuthorityByID)]
        [TdbCacheKey(0)]
        public async Task<AuthorityAgg?> GetAuthorityAggAsync(long authorityID)
        {
            var authorityInfo = await this.GetByIdAsync(authorityID);
            if (authorityInfo is null)
            {
                return null;
            }

            //转为聚合
            var authorityAgg = DBMapper.Map<AuthorityInfo, AuthorityAgg>(authorityInfo);
            return authorityAgg;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">权限聚合</param>
        [TdbRemoveCacheHash(AccountCst.CacheKey.HashAuthorityByID)]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task SaveAsync(AuthorityAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<AuthorityAgg, AuthorityInfo>(agg);

            //保存
            await this.InsertOrUpdateAsync(info);
        }

        #endregion
    }
}
