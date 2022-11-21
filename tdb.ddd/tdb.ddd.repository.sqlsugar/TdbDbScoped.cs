using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.contracts;

namespace tdb.ddd.repository.sqlsugar
{
    /// <summary>
    /// 获取数据库上下文
    /// </summary>
    internal class TdbDbScoped
    {
        /// <summary>
        /// 服务供应商
        /// </summary>
        private static IServiceProvider? Provider { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="provider">服务供应商</param>
        public static void Init(IServiceProvider provider)
        {
            Provider = provider;
        }

        /// <summary>
        /// http请求数据库上下文缓存
        /// （key：http请求唯一标识）
        /// </summary>
        private static ConcurrentDictionary<string, SqlSugarClient> dicHttpDbContext = new ConcurrentDictionary<string, SqlSugarClient>();

        /// <summary>
        /// 获取数据库上下文
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetScopedContext()
        {
            if (Provider == null)
            {
                throw new TdbException("未配置容器，请先调用方法[UseTdbSqlSugar(this IServiceProvider provider)]配置容器");
            }

            //尝试获取当前请求访问
            var httpContextAccessor = Provider.GetRequiredService<IHttpContextAccessor>();
            if (httpContextAccessor?.HttpContext?.Response != null)
            {
                //http请求唯一标识
                var traceIdentifier = httpContextAccessor.HttpContext.TraceIdentifier;
                //注册http请求完成时回调方法
                httpContextAccessor.HttpContext.Response.OnCompleted(OnHttpResponseCompleted, traceIdentifier);

                //获取数据库上下文
                return dicHttpDbContext.GetOrAdd(traceIdentifier, traceIdentifier => DbScoped.SugarScope.ScopedContext);
            }

            //获取数据库上下文
            return DbScoped.SugarScope.ScopedContext;
        }

        /// <summary>
        /// http请求完成时
        /// </summary>
        /// <param name="traceIdentifier">http请求唯一标识</param>
        /// <returns></returns>
        private static async Task OnHttpResponseCompleted(object traceIdentifier)
        {
            //从缓存中移除
            dicHttpDbContext.Remove(traceIdentifier.ToStr(), out _);
            await Task.CompletedTask;
        }
    }
}
