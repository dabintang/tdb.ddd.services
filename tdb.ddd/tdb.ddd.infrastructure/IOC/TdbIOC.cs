using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// IOC容器
    /// </summary>
    public class TdbIOC
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
        /// 获取服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns></returns>
        public static T? GetService<T>()
        {
            if (Provider == null)
            {
                throw new TdbException("未配置IOC容器，请先调用方法[TdbIOC.Init(IServiceProvider provider)]配置IOC容器");
            }

            //获取当前请求访问器
            var httpContextAccessor = GetHttpContextAccessor();
            if (httpContextAccessor?.HttpContext?.RequestServices == null)
            {
                return Provider.GetService<T>();
            }

            return httpContextAccessor.HttpContext.RequestServices.GetService<T>();
        }

        /// <summary>
        /// 获取当前请求访问器
        /// </summary>
        /// <returns></returns>
        public static IHttpContextAccessor? GetHttpContextAccessor()
        {
            if (Provider == null)
            {
                //throw new TdbException("未配置IOC容器，请先调用方法[TdbIOC.Init(IServiceProvider provider)]配置IOC容器");
                return null;
            }

            return Provider.GetRequiredService<IHttpContextAccessor>();
        }
    }
}
