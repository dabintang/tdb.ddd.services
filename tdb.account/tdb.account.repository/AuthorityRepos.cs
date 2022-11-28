﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.Authority;
using tdb.account.domain.Authority.Aggregate;
using tdb.account.domain.Role.Aggregate;
using tdb.account.repository.DBEntity;
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;
using static tdb.account.domain.contracts.Const.Cst;

namespace tdb.account.repository
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
        public async Task<AuthorityAgg> GetAuthorityAggAsync(int authorityID)
        {
            //获取权限信息
            var authorityInfo = await TdbCache.Ins.CacheShellAsync(this.CacheKeyAuthorityInfo(authorityID), TimeSpan.FromDays(1), async () => await this.GetByIdAsync(authorityID));

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
            await this.AsUpdateable(info).ExecuteCommandAsync();
            //移除缓存
            TdbCache.Ins.Del(this.CacheKeyAuthorityInfo(info.ID));
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取【权限信息】缓存key
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        /// <returns></returns>
        private string CacheKeyAuthorityInfo(int authorityID)
        {
            return $"ReposAuthorityInfo_{authorityID}";
        }

        #endregion
    }
}
