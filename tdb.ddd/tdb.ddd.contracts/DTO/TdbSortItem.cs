using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 排序项
    /// </summary>
    public class TdbSortItem
    {
        /// <summary>
        /// 排序字段名
        /// </summary>
        public string? FieldName { get; set; }

        /// <summary>
        /// 排序方式（1：升序；2：降序）
        /// </summary>
        public EnmSort SortCode { get; set; }

        /// <summary>
        /// 排序方式（1：升序；2：降序）
        /// </summary>
        public enum EnmSort
        {
            /// <summary>
            /// 升序
            /// </summary>
            Asc = 1,

            /// <summary>
            /// 降序
            /// </summary>
            Desc = 2
        }
    }
}
