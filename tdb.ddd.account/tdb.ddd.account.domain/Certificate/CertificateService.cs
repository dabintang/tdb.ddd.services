using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common.Crypto;
using tdb.ddd.account.domain.Certificate.Aggregate;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Certificate
{
    /// <summary>
    /// 凭证领域服务
    /// </summary>
    public class CertificateService
    {
        #region 仓储

        private ICertificateRepos? _certificateRepos;
        /// <summary>
        /// 凭证仓储
        /// </summary>
        private ICertificateRepos CertificateRepos
        {
            get
            {
                this._certificateRepos ??= TdbIOC.GetService<ICertificateRepos>();
                if (this._certificateRepos is null)
                {
                    throw new TdbException("凭证仓储接口未实现");
                }

                return this._certificateRepos;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 获取凭证聚合
        /// </summary>
        /// <param name="certificateTypeCode">凭证类型</param>
        /// <param name="credentials">凭证内容</param>
        /// <returns></returns>
        public async Task<CertificateAgg?> GetCertificateAggAsync(EnmCertificateType certificateTypeCode, string credentials)
        {
            if (credentials.Length > 32)
            {
                credentials = EncryptHelper.Md5(credentials);
            }

            return await this.CertificateRepos.GetCertificateAggAsync((byte)certificateTypeCode, credentials);
        }

        #endregion
    }
}
