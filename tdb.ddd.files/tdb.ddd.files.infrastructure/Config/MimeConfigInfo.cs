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
    public class MimeConfigInfo
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// mime配置信息集合
        /// </summary>
        public List<MimeItemConfigInfo> Mimes { get; set; }

        /// <summary>
        /// 根据扩展名查找
        /// </summary>
        /// <param name="extensionName"></param>
        /// <returns></returns>
        public string FindContentType(string? extensionName)
        {
            if (this.Mimes is null || string.IsNullOrWhiteSpace(extensionName))
            {
                return "text/plain";
            }

            extensionName = extensionName.TrimStart('.').ToLower();

            var mime = this.Mimes.Find(m => m.ExtensionNames?.Contains(extensionName) ?? false);
            return mime?.ContentType ?? "text/plain";
        }

        #region 内部类

        /// <summary>
        /// mime配置信息
        /// </summary>
        public class MimeItemConfigInfo
        {
            /// <summary>
            /// 内容类型
            /// </summary>
            public string? ContentType { get; set; }

            /// <summary>
            /// 扩展名集合
            /// </summary>
            public List<string>? ExtensionNames { get; set; }
        }
        #endregion

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}
