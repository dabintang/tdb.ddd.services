using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.account.domain.contracts.Enum
{
    /// <summary>
    /// 信息状态（1：可用的；2：禁用的）
    /// </summary>
    public enum EnmInfoStatus : byte
    {
        /// <summary>
        /// 1：激活
        /// </summary>
        Enable = 1,

        /// <summary>
        /// 2：禁用
        /// </summary>
        Disable = 2,
    }
}
