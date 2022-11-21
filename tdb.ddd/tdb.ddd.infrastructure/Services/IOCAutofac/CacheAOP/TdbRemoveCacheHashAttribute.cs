using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 清除hash类型缓存特性
    /// </summary>
    public class TdbRemoveCacheHashAttribute : TdbCacheBaseAttribute
    {
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">key</param>
        public TdbRemoveCacheHashAttribute(string key)
        {
            this.Key = key;
        }
    }
}
