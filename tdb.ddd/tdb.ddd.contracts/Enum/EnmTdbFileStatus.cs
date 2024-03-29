﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 文件状态（1：临时文件；2：正式文件）
    /// </summary>
    public enum EnmTdbFileStatus : byte
    {
        /// <summary>
        /// 1：临时文件
        /// </summary>
        Temp = 1,

        /// <summary>
        /// 2：正式文件
        /// </summary>
        Formal = 2
    }
}
