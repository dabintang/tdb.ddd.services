using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.infrastructure.Common.Http
{
    /// <summary>
    /// 下载文件结果
    /// </summary>
    public class TdbDownloadFileRes
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = "";

        /// <summary>
        /// 内容类型
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// 内容长度
        /// </summary>
        public long? ContentLength { get; set; }

        /// <summary>
        /// 内容流
        /// </summary>
        public Stream? ContentStream { get; set; }
    }
}
