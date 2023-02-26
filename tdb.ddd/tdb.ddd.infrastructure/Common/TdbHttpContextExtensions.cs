using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// HttpContext扩展类
    /// </summary>
    public static class TdbHttpContextExtensions
    {

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

        /// <summary>
        /// 获取头部身份认证信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static AuthenticationHeaderValue? GetAuthenticationHeaderValue(this HttpContext context)
        {
            if (context.Request.Headers != null)
            {
                if (context.Request.Headers.TryGetValue("Authorization", out StringValues strAuthorization))
                {
                    return AuthenticationHeaderValue.Parse(strAuthorization);
                }
            }

            return null;
        }
    }
}
