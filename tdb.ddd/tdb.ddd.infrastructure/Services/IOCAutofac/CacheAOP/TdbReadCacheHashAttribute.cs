using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 读取hash类型缓存特性
    /// </summary>
    public class TdbReadCacheHashAttribute : TdbCacheBaseAttribute
    {
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 缓存天数（默认1天，即缓存会在第2天指定时间点失效）
        /// </summary>
        public int TimeoutDays { get; set; } = 1;

        /// <summary>
        /// 过期时间（格式：HH:mm:ss，默认值："01:00:00"，即缓存会在缓存天数+此时间点失效）
        /// </summary>
        public string ExpireAtTime { get; set; } = "01:00:00";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">key</param>
        public TdbReadCacheHashAttribute(string key)
        {
            this.Key = key;
        }
    }
}
