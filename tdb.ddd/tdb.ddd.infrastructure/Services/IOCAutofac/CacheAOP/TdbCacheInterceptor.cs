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
        /// 缓存
        /// </summary>
        private readonly ITdbCache cache;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TdbCacheInterceptor()
        {
            this.cache = TdbCache.Ins;
        }

        /// <summary>
        /// 拦截方法
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            //尝试获取缓存特效
            var attrCache = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(TdbCacheBaseAttribute), true).FirstOrDefault();
            //未加缓存特效
            if (attrCache == null)
            {
                //执行被拦截的方法
                invocation.Proceed();
                return;
            }

            //获取key
            var keyFields = GetKey(invocation);

            #region 读取key-value类型缓存

            if (attrCache is TdbReadCacheStringAttribute attrReadString)
            {
                var key = $"{attrReadString.KeyPrefix}{keyFields}";
                var returnValue = this.cache.CacheShell(key, TimeSpan.FromSeconds(attrReadString.TimeoutSeconds), () =>
                {
                    //执行被拦截的方法
                    invocation.Proceed();
                    //return invocation.ReturnValue;
                    return invocation.ReturnValue.SerializeJson();
                });
                //invocation.ReturnValue = returnValue;
                if (returnValue == null)
                {
                    invocation.ReturnValue = null;
                }
                else
                {
                    invocation.ReturnValue = returnValue.DeserializeJson(invocation.MethodInvocationTarget.ReturnType);
                }

                return;
            }

            #endregion

            #region 读取hash类型缓存

            if (attrCache is TdbReadCacheHashAttribute attrReadHash)
            {
                //过期时间点
                var expireAt = DateTime.Today.AddDays(attrReadHash.TimeoutDays);
                expireAt = Convert.ToDateTime($"{expireAt:yyyy-MM-dd} {attrReadHash.ExpireAtTime}", new DateTimeFormatInfo() { FullDateTimePattern = "yyyy-MM-dd HH:mm:ss" });
                //过期时间
                var expire = expireAt - DateTime.Now;
                if (expire.TotalSeconds <= 0)
                {
                    //执行被拦截的方法
                    invocation.Proceed();
                    return;
                }

                var returnValue = this.cache.HCacheShell(attrReadHash.Key, expire, keyFields, () =>
                {
                    //执行被拦截的方法
                    invocation.Proceed();
                    return invocation.ReturnValue.SerializeJson();
                });

                if (returnValue == null)
                {
                    invocation.ReturnValue = null;
                }
                else
                {
                    invocation.ReturnValue = returnValue.DeserializeJson(invocation.MethodInvocationTarget.ReturnType);
                }

                return;
            }

            #endregion

            #region 清除key-value类型缓存

            if (attrCache is TdbRemoveCacheStringAttribute attrRemoveString)
            {
                //执行被拦截的方法
                invocation.Proceed();

                var key = $"{attrRemoveString.KeyPrefix}{keyFields}";
                this.cache.Del(key);

                return;
            }

            #endregion

            #region 清除hash类型缓存

            if (attrCache is TdbRemoveCacheHashAttribute attrRemoveHash)
            {
                //执行被拦截的方法
                invocation.Proceed();

                this.cache.HDel(attrRemoveHash.Key, keyFields);

                return;
            }

            #endregion

            //执行被拦截的方法
            invocation.Proceed();
        }

        /// <summary>
        /// 获取key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private static string GetKey(IInvocation invocation)
        {
            var sb = new StringBuilder();
            var attrKeys = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(TdbCacheKeyAttribute), true).Select(m => m as TdbCacheKeyAttribute);
            foreach (var attrKey in attrKeys)
            {
                if (attrKey == null) continue;

                //获取参数值
                var param = invocation.GetArgumentValue(attrKey.ParamIndex);

                //直接获取
                if (string.IsNullOrWhiteSpace(attrKey.FromPropertyName))
                {
                    sb.Append(param.ToStr());
                }
                //从属性获取
                else
                {
                    //属性不存在
                    if (CommHelper.IsExistPropertyOrField(param, attrKey.FromPropertyName) == false)
                    {
                        throw new TdbException($"[缓存拦截器]找不到属性：{param.GetType().Name}.{attrKey.FromPropertyName}");
                    }

                    var paramValue = CommHelper.ReflectGet(param, attrKey.FromPropertyName);
                    sb.Append(paramValue.ToStr());
                }
            }

            return sb.ToString();
        }
    }
}
