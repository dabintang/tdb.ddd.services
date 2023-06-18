using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.files.infrastructure
{
    /// <summary>
    /// 文件服务常量
    /// </summary>
    public class FilesCst
    {
        /// <summary>
        /// 缓存key前缀
        /// </summary>
        public class CacheKey
        {
            /// <summary>
            /// hash形式缓存文件信息
            /// </summary>
            public const string HashFileByID = "HashFileByID";
        }
    }
}
