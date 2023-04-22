using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 排序项
    /// </summary>
    public class TdbSortItem<T> where T : struct, System.Enum
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public T FieldCode { get; set; }

        /// <summary>
        /// 排序方式（1：升序；2：降序）
        /// </summary>
        public EnmTdbSort SortCode { get; set; }
    }

    /// <summary>
    /// 排序方式（1：升序；2：降序）
    /// </summary>
    public enum EnmTdbSort
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
