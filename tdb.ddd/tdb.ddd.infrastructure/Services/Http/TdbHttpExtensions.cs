using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// http扩展类
    /// </summary>
    public static class TdbHttpExtensions
    {
        /// <summary>
        /// 添加http client服务
        /// </summary>
        /// <param name="services">服务容器</param>
        public static void AddTdbHttpClient(this IServiceCollection services)
        {
            //            services.AddTransient<TdbHttpClientLogHandler>();
            //#pragma warning disable CS8603 // 可能返回 null 引用。
            //            services.AddHttpClient(TdbHttpClient.DefaultName).AddHttpMessageHandler(h => h.GetService<TdbHttpClientLogHandler>());
            //#pragma warning restore CS8603 // 可能返回 null 引用。

            services.AddHttpClient(TdbHttpClient.DefaultName);
        }
    }
}
