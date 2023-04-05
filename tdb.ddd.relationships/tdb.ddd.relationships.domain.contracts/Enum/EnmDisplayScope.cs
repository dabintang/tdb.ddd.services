using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.relationships.domain.contracts.Enum
{
    /// <summary>
    /// 展示范围类型（1：全部人际圈可见；2：仅授权人际圈可见）
    /// </summary>
    public enum EnmDisplayScope : byte
    {
        /// <summary>
        /// 全部人际圈可见
        /// </summary>
        All = 1,

        /// <summary>
        /// 仅授权人际圈可见
        /// </summary>
        Approved = 2
    }
}
