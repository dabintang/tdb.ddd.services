using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.relationships.domain.contracts.Enum
{
    /// <summary>
    /// 性别（1：男；2：女；3：未知）
    /// </summary>
    public enum EnmGender : byte
    {
        /// <summary>
        /// 1：男
        /// </summary>
        Male = 1,

        /// <summary>
        /// 2：女
        /// </summary>
        Female = 2,

        /// <summary>
        /// 3：未知
        /// </summary>
        Unknown = 3
    }
}
