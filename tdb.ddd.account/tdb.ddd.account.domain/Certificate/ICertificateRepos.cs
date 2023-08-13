using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Certificate.Aggregate;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.account.domain.User.Aggregate;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Certificate
{
    /// <summary>
    /// 凭证仓储接口
    /// </summary>
    public interface ICertificateRepos : ITdbIOCScoped, ITdbIOCIntercept
    {
        /// <summary>
        /// 获取凭证聚合
        /// </summary>
        /// <param name="certificateTypeCode">凭证类型</param>
        /// <param name="credentials">凭证内容</param>
        /// <returns></returns>
        Task<CertificateAgg?> GetCertificateAggAsync(byte certificateTypeCode, string credentials);

        /// <summary>
        /// 获取凭证绑定的用户ID集合
        /// </summary>
        /// <param name="certificateID">凭证ID</param>
        /// <returns></returns>
        Task<List<long>> GetUserIDsAsync(long certificateID);

        /// <summary>
        /// 绑定用户
        /// </summary>
        /// <param name="certificateID">凭证ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task BindingUserAsync(long certificateID, long userID);

        /// <summary>
        /// 解绑所有用户
        /// </summary>
        /// <param name="certificateID">凭证ID</param>
        /// <returns></returns>
        Task UnbindingAllUserAsync(long certificateID);

        /// <summary>
        /// 保存凭证
        /// </summary>
        /// <param name="agg">凭证聚合</param>
        Task SaveAsync(CertificateAgg agg);

        /// <summary>
        /// 删除凭证
        /// </summary>
        /// <param name="agg">凭证聚合</param>
        /// <returns></returns>
        Task DeleteAsync(CertificateAgg agg);
    }
}
