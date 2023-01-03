using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.files.domain.contracts.Enum
{
    /// <summary>
    /// 访问级别(1：仅创建者；2：授权；3：公开)
    /// </summary>
    public enum EnmAccessLevel : byte
    {
        /// <summary>
        /// 1：仅创建者
        /// </summary>
        OnlyCreator = 1,

        /// <summary>
        /// 2：授权
        /// </summary>
        Authorization = 2,

        /// <summary>
        /// 3：公开
        /// </summary>
        Public = 3
    }
}
