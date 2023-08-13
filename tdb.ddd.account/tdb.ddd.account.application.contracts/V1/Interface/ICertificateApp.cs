using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.application.contracts.V1.DTO;
using tdb.ddd.account.application.contracts.V1.DTO.Certificate;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.account.application.contracts.V1.Interface
{
    /// <summary>
    /// 凭证应用接口
    /// </summary>
    public interface ICertificateApp : ITdbIOCScoped
    {
        /// <summary>
        /// 凭证登录
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<UserLoginRes>> LoginAsync(CertificateLoginReq req);

        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> AddCertificateAsync(TdbOperateReq<AddCertificateReq> req);

        /// <summary>
        /// 删除凭证
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> DeleteCertificateAsync(TdbOperateReq<DeleteCertificateReq> req);
    }
}
