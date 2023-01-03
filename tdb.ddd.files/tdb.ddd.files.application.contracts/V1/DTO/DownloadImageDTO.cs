using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.ddd.files.application.contracts.V1.DTO
{
    /// <summary>
    /// 下载图片 请求条件
    /// </summary>
    public class DownloadImageReq
    {
        /// <summary>
        /// [必须]文件ID
        /// </summary>
        [TdbHashIDModelBinder]
        [TdbRequired("文件ID")]
        public long ID { get; set; }

        /// <summary>
        /// [可选]宽度（如果不传，则按高度比例缩放）
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// [可选]高度（如果不传，则按宽度比例缩放）
        /// </summary>
        public int Height { get; set; }
    }
}
