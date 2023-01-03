using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.files.infrastructure.Config
{
    /// <summary>
    /// mime配置
    /// </summary>
    public class MimeConfig
    {
        /// <summary>
        /// mime配置信息集合
        /// </summary>
        public List<MimeConfigInfo> Mimes { get; set; }

        /// <summary>
        /// 根据扩展名查找
        /// </summary>
        /// <param name="extensionName"></param>
        /// <returns></returns>
        public string FindContentType(string extensionName)
        {
            if (this.Mimes == null || string.IsNullOrWhiteSpace(extensionName))
            {
                return "text/plain";
            }

            extensionName = extensionName.TrimStart('.').ToLower();

            var mime = this.Mimes.Find(m => m.ExtensionNames.Contains(extensionName));
            return mime?.ContentType ?? "text/plain";
        }
    }

    /// <summary>
    /// mime配置信息
    /// </summary>
    public class MimeConfigInfo
    {
        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 扩展名集合
        /// </summary>
        public List<string> ExtensionNames { get; set; }
    }
}
