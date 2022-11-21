using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 分页结果包装器
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public class TdbPageRes<T> : TdbRes<List<T>>
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msgInfo">消息</param>
        /// <param name="data">结果</param>
        /// <param name="totalCount">总条数</param>
        public TdbPageRes(TdbResMsgInfo msgInfo, List<T> data, int totalCount) : base(msgInfo, data)
        {
            this.TotalCount = totalCount;
        }
    }
}
