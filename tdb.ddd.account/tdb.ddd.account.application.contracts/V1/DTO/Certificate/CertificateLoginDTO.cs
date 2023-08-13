using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.application.contracts;

namespace tdb.ddd.account.application.contracts.V1.DTO.Certificate
{
    /// <summary>
    /// 凭证登录 请求参数
    /// </summary>
    public class CertificateLoginReq
    {
        /// <summary>
        /// [必须]凭证类型
        /// </summary>
        [TdbRequired("凭证类型")]
        [TdbEnumDataType(typeof(EnmCertificateType), "凭证类型不正确")]
        public EnmCertificateType CertificateTypeCode { get; set; }

        /// <summary>
        /// [必须]凭证内容
        /// </summary>
        [TdbRequired("凭证内容")]
        [TdbStringLength("凭证内容", 255)]
        public string Credentials { get; set; } = "";

        /// <summary>
        /// [必须]客户端IP（内部参数，webapi上看不到）
        /// </summary>
        [JsonIgnore]
        public string ClientIP { get; set; } = "";
    }
}
