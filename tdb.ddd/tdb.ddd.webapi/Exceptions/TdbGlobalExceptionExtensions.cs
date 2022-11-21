using Microsoft.AspNetCore.Mvc;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 全局异常处理
    /// </summary>
    public static class TdbGlobalExceptionExtensions
    {
        /// <summary>
        /// 添加全局异常处理过滤器
        /// </summary>
        /// <param name="options"></param>
        public static void AddTdbGlobalExceptionFilter(this MvcOptions options)
        {
            options.Filters.Add<TdbGlobalExceptionFilter>();
        }
    }
}
