using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Certificate;
using tdb.ddd.account.domain.Certificate.Aggregate;
using tdb.ddd.account.infrastructure;
using tdb.ddd.account.repository.DBEntity;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.account.repository
{
    /// <summary>
    /// 凭证存储
    /// </summary>
    [Intercept(typeof(TdbCacheInterceptor))]
    public class CertificateRepos : TdbRepository<CertificateInfo>, ICertificateRepos
    {
        #region 实现接口

        /// <summary>
        /// 获取凭证聚合
        /// </summary>
        /// <param name="certificateTypeCode">凭证类型</param>
        /// <param name="credentials">凭证内容</param>
        /// <returns></returns>
        [TdbReadCacheHash(AccountCst.CacheKey.HashCertificateByTypeAndContext)]
        [TdbCacheKey(0)]
        [TdbCacheKey(1)]
        public async Task<CertificateAgg?> GetCertificateAggAsync(byte certificateTypeCode, string credentials)
        {
            var info = await this.GetFirstAsync(m => m.CertificateTypeCode == certificateTypeCode && m.Credentials == credentials);
            if (info is null)
            {
                return null;
            }

            var agg = DBMapper.Map<CertificateInfo, CertificateAgg>(info);
            return agg;
        }

        /// <summary>
        /// 获取凭证绑定的用户ID集合
        /// </summary>
        /// <param name="certificateID">凭证ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(AccountCst.CacheKey.HashCertificateUserIDsByCertificateID)]
        [TdbCacheKey(0)]
        public async Task<List<long>> GetUserIDsAsync(long certificateID)
        {
            var list = await this.Change<CertificateUserConfig>().AsQueryable().Where(m => m.CertificateID == certificateID).Select(m => m.UserID).ToListAsync();
            return list ?? new List<long>();
        }

        /// <summary>
        /// 绑定用户
        /// </summary>
        /// <param name="certificateID">凭证ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [TdbRemoveCacheHash(AccountCst.CacheKey.HashCertificateUserIDsByCertificateID)]
        [TdbCacheKey(0)]
        public async Task BindingUserAsync(long certificateID, long userID)
        {
            var config = new CertificateUserConfig()
            {
                CertificateID = certificateID,
                UserID = userID
            };

            await this.Change<CertificateUserConfig>().InsertOrUpdateAsync(config);
        }

        /// <summary>
        /// 解绑所有用户
        /// </summary>
        /// <param name="certificateID">凭证ID</param>
        /// <returns></returns>
        [TdbRemoveCacheHash(AccountCst.CacheKey.HashCertificateUserIDsByCertificateID)]
        [TdbCacheKey(0)]
        public async Task UnbindingAllUserAsync(long certificateID)
        {
            await this.Change<CertificateUserConfig>().AsDeleteable().Where(m => m.CertificateID == certificateID).ExecuteCommandAsync();
        }

        /// <summary>
        /// 保存凭证
        /// </summary>
        /// <param name="agg">凭证聚合</param>
        [TdbRemoveCacheHash(AccountCst.CacheKey.HashCertificateByTypeAndContext)]
        [TdbCacheKey(0, FromPropertyName = "CertificateTypeCode")]
        [TdbCacheKey(0, FromPropertyName = "Credentials")]
        public async Task SaveAsync(CertificateAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<CertificateAgg, CertificateInfo>(agg);
            //更新或保存
            await this.InsertOrUpdateAsync(info);
        }

        /// <summary>
        /// 删除凭证
        /// </summary>
        /// <param name="agg">凭证聚合</param>
        /// <returns></returns>
        [TdbRemoveCacheHash(AccountCst.CacheKey.HashCertificateByTypeAndContext)]
        [TdbCacheKey(0, FromPropertyName = "CertificateTypeCode")]
        [TdbCacheKey(0, FromPropertyName = "Credentials")]
        public async Task DeleteAsync(CertificateAgg agg)
        {
            await this.AsDeleteable().Where(m => m.ID == agg.ID).ExecuteCommandAsync();
        }

        #endregion
    }
}
