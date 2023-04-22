using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 分页请求基类
    /// </summary>
    public class TdbPageReqBase<T> : ITdbSortReq<T> where T : struct, Enum
    {
        /// <summary>
        /// 页码（从1开始）
        /// </summary>
        public int PageNO { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 排序项集合
        /// </summary>
        public virtual List<TdbSortItem<T>>? LstSortItem { get; set; }
    }
}
