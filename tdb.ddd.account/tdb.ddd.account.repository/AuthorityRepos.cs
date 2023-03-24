using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Authority;
using tdb.ddd.account.domain.Authority.Aggregate;
using tdb.ddd.account.repository.DBEntity;
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.account.repository
{
    /// <summary>
    /// 权限仓储
    /// </summary>
    public class AuthorityRepos : TdbRepository<AuthorityInfo>, IAuthorityRepos
    {
        #region 实现接口

        /// <summary>
        /// 获取权限聚合
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        /// <returns></returns>
        public async Task<AuthorityAgg?> GetAuthorityAggAsync(long authorityID)
        {
            var authorityInfo = await TdbCache.Ins.CacheShellAsync(CacheKeyAuthorityInfo(authorityID), TimeSpan.FromDays(1), async () => await this.GetByIdAsync(authorityID));
            if (authorityInfo is null)
            {
                return null;
            }

            //转为聚合
            var authorityAgg = DBMapper.Map<AuthorityInfo, AuthorityAgg>(authorityInfo);

            return authorityAgg;
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">权限聚合</param>
        public async Task SaveChangedAsync(AuthorityAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<AuthorityAgg, AuthorityInfo>(agg);

            //保存
            await this.InsertOrUpdateAsync(info!);
            //移除缓存
            TdbCache.Ins.Del(CacheKeyAuthorityInfo(info!.ID));
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取【权限信息】缓存key
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        /// <returns></returns>
        private static string CacheKeyAuthorityInfo(long authorityID)
        {
            return $"ReposAuthorityInfo_{authorityID}";
        }

        #endregion
    }
}
