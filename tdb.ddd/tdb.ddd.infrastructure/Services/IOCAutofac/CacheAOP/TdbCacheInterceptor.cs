using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using tdb.common;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 缓存拦截器
    /// </summary>
    public class TdbCacheInterceptor : IInterceptor
    {
        /// <summary>
        /// 缓存拦截器（异步方法）
        /// </summary>
        private readonly TdbCacheInterceptorAsync _interceptorAsync;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="interceptorAsync">缓存拦截器（异步方法）</param>
        public TdbCacheInterceptor(TdbCacheInterceptorAsync interceptorAsync)
        {
            this._interceptorAsync = interceptorAsync;
        }

        /// <summary>
        /// 拦截方法
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            this._interceptorAsync.ToInterceptor().Intercept(invocation);
        }
    }
}
