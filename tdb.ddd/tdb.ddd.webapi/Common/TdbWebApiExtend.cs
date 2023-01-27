using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using tdb.common;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 通用帮助/扩展类
    /// </summary>
    public static class TdbWebApiExtend
    {
        /// <summary>
        /// 获取接口指定的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T? GetAttribute<T>(this FilterContext context) where T : Attribute
        {
            if (context == null || context.ActionDescriptor == null)
            {
                return null;
            }

            T? attr = null;

            //类和方法中的特性
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                //取方法上的特性
                attr = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
                if (attr != null)
                {
                    return attr;
                }

                //取控制器上的特性
                attr = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
                if (attr != null)
                {
                    return attr;
                }
            }

            //取过滤器管道中的特性
            attr = context.Filters.Where(m => m is T).FirstOrDefault() as T;
            return attr;
        }

        /// <summary>
        /// 获取接口指定的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(this FilterContext context) where T : Attribute
        {
            var list = new List<T>();

            if (context == null || context.ActionDescriptor == null)
            {
                return list;
            }

            //类和方法中的特性
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                //取方法上的特性
                var lstMethodAttr = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(T), true).Cast<T>();
                list.AddRange(lstMethodAttr);

                //取控制器上的特性
                var lstCtlAttr = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(T), true).Cast<T>();
                list.AddRange(lstCtlAttr);
            }

            //取过滤器管道中的特性
            var lstFilterAttr = context.Filters.Where(m => m is T).Cast<T>();
            list.AddRange(lstFilterAttr);

            return list;
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="context">Encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns></returns>
        public static string GetClientIP(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();//负载均衡
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection?.RemoteIpAddress?.ToString() ?? "";
            }

            return ip;
        }

        /// <summary>
        /// 获取头部参数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetHeaderValue(this HttpContext context, string key)
        {
            if (context.Request.Headers != null)
            {
                if (context.Request.Headers.TryGetValue(key, out StringValues val))
                {
                    return val.ToString();
                }
            }

            return string.Empty;
        }
    }
}
