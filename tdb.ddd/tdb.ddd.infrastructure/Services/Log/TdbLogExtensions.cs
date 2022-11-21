using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 日志扩展
    /// </summary>
    public static class TdbLogExtensions
    {
        /// <summary>
        /// 添加nlog日志服务（文本）
        /// </summary>
        /// <param name="services">服务容器</param>
        public static void AddTdbNLogger(this IServiceCollection services)
        {
            AddTdbLogger(services, () => new TdbNLog());
        }

        /// <summary>
        /// 添加日志服务（指定服务）
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="getTdbLog">获取服务方法</param>
        public static void AddTdbLogger(this IServiceCollection services, Func<ITdbLog> getTdbLog)
        {
            var log = getTdbLog();
            TdbLogger.InitLog(log);

            services.AddSingleton(typeof(ITdbLog), log);
        }
    }
}
