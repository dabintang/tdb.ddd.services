using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.account.domain.contracts.Enum
{
    /// <summary>
    /// 凭证类型（1：指纹）
    /// </summary>
    public enum EnmCertificateType : byte
    {
        /// <summary>
        /// 1：指纹
        /// </summary>
        Fingerprint = 1
    }
}
