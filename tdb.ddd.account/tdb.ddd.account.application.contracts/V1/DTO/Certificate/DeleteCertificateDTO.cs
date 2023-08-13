using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.application.contracts;

namespace tdb.ddd.account.application.contracts.V1.DTO.Certificate
{
    /// <summary>
    /// 删除凭证条件
    /// </summary>
    public class DeleteCertificateReq
    {
        /// <summary>
        /// 凭证类型
        /// </summary>
        [TdbRequired("凭证类型")]
        [TdbEnumDataType(typeof(EnmCertificateType), "凭证类型不正确")]
        public EnmCertificateType CertificateTypeCode { get; set; }

        /// <summary>
        /// 凭证内容
        /// </summary>
        [TdbRequired("凭证内容")]
        [TdbStringLength("凭证内容", 255)]
        public string Credentials { get; set; } = "";
    }
}
