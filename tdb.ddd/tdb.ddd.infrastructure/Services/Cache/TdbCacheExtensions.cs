using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 缓存扩展
    /// </summary>
    public static class TdbCacheExtensions
    {
        /// <summary>
        /// 添加redis缓存服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionStrings">连接字符串集合</param>
        public static void AddTdbRedisCache(this IServiceCollection services, params string[] connectionStrings)
        {
            AddTdbCache(services, () => new TdbRedisCache(connectionStrings));
        }

        /// <summary>
        /// 添加内存缓存服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option">内存缓存配置</param>
        public static void AddTdbMemoryCache(this IServiceCollection services, MemoryCacheOptions? option = null)
        {
            AddTdbCache(services, () => new TdbMemoryCache(option));
        }

        /// <summary>
        /// 添加缓存服务（指定服务）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="getTdbCache">获取缓存服务</param>
        public static void AddTdbCache(this IServiceCollection services, Func<ITdbCache> getTdbCache)
        {
            var cache = getTdbCache();
            TdbCache.InitCache(cache);

            services.AddSingleton(typeof(ITdbCache), cache);
        }
    }
}
