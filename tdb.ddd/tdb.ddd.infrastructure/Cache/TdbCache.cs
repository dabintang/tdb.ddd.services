using System;
using System.Collections.Generic;
using System.Text;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// 缓存
    /// </summary>
    public class TdbCache
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private static ITdbCache? cache = null;

        /// <summary>
        /// 缓存实例
        /// </summary>
        public static ITdbCache Ins
        {
            get
            {
                if (cache == null)
                {
                    throw new TdbException("未配置缓存服务，请先在IServiceCollection添加缓存服务");
                }

                return cache;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cache">缓存服务</param>
        internal static void InitCache(ITdbCache cache)
        {
            TdbCache.cache = cache;
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private TdbCache()
        {
        }
    }
}
