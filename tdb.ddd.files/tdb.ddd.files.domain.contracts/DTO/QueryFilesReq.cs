using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.files.domain.contracts.DTO
{
    /// <summary>
    /// 查询文件 条件参数
    /// </summary>
    public class QueryFilesReq : TdbPageReqBase<QueryFilesReq.EnmSortField>
    {
        /// <summary>
        /// [可选]文件状态（1：临时文件；2：正式文件）
        /// </summary>
        public byte? FileStatusCode { get; set; }

        /// <summary>
        /// [可选]起始创建时间（大于等于）
        /// </summary>
        public DateTime? StartCreateTime { get; set; }

        /// <summary>
        /// [可选]截止创建时间（小于等于）
        /// </summary>
        public DateTime? EndCreateTime { get; set; }

        /// <summary>
        /// 支持排序的字段（暂不支持排序）
        /// </summary>
        public enum EnmSortField
        {
            ///// <summary>
            ///// 文件ID
            ///// </summary>
            //ID = 1,

            ///// <summary>
            ///// 文件名
            ///// </summary>
            //Name = 2,

            ///// <summary>
            ///// 字节数
            ///// </summary>
            //Size = 3
        }
    }
}
