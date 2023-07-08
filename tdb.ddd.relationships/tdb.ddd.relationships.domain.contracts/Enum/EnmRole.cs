using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.relationships.domain.contracts.Enum
{
    /// <summary>
    /// 角色(1：普通成员；2：管理员)
    /// </summary>
    public enum EnmRole : byte
    {
        /// <summary>
        /// 1：普通成员
        /// </summary>
        Member = 1,

        /// <summary>
        /// 2：管理员
        /// </summary>
        Admin = 2
    }
}
