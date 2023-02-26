using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.files.application.contracts.V1.DTO
{
    /// <summary>
    /// 删除临时文件 条件参数
    /// </summary>
    public class DeleteTempFilesReq
    {
        /// <summary>
        /// [可选]创建时间大于等于该时间的临时文件将被删除
        /// </summary>
        public DateTime? StartCreateTime { get; set; }

        /// <summary>
        /// [可选]创建时间小于等于该时间的临时文件将被删除
        /// </summary>
        public DateTime? EndCreateTime { get; set; }
    }

    /// <summary>
    /// 删除临时文件 结果
    /// </summary>
    public class DeleteTempFilesRes
    {
        /// <summary>
        /// 删除文件数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 删除详情
        /// </summary>
        public List<DeleteTempFileRes> Details { get; set; } = new List<DeleteTempFileRes>();
    }

    /// <summary>
    /// 删除临时文件 结果
    /// </summary>
    public class DeleteTempFileRes
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
    }
}
