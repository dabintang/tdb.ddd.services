using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.ddd.files.application.contracts.V1.DTO
{
    /// <summary>
    /// 下载文件 请求条件
    /// </summary>
    public class DownloadFileReq
    {
        /// <summary>
        /// [必须]文件ID
        /// </summary>
        [TdbHashIDModelBinder]
        [TdbRequired("文件ID")]
        public long ID { get; set; }
    }

    /// <summary>
    /// 下载文件结果
    /// </summary>
    public class DownloadFileRes
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }

        /// <summary>
        /// 文件名（含后缀）
        /// </summary>           
        public string Name { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 文件数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
