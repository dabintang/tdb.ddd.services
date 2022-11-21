using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 排序条件接口
    /// </summary>
    public interface ITdbSortReq
    {
        /// <summary>
        /// 排序项集合
        /// </summary>
        List<TdbSortItem>? LstSortItem { get; set; }
    }
}
