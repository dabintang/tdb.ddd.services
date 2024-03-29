﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.domain.contracts.Personnel
{
    /// <summary>
    /// 查询人员参数
    /// </summary>
    public class QueryPersonnelParam : TdbPageReqBase<QueryPersonnelParam.EnmSortField>
    {
        /// <summary>
        /// [可选]创建者ID
        /// </summary>
        public long? CreatorID { get; set; }

        /// <summary>
        /// [可选]人际圈ID
        /// </summary>
        public long? CircleID { get; set; }

        /// <summary>
        /// 排序字段（1：人员ID；2：姓名）
        /// </summary>
        public enum EnmSortField
        {
            /// <summary>
            /// 人员ID
            /// </summary>
            ID = 1,

            /// <summary>
            /// 姓名
            /// </summary>
            Name
        }
    }
}
