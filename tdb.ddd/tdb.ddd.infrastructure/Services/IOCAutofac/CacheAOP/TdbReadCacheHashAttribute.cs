using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using tdb.common;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 读取hash类型缓存特性
    /// </summary>
    public class TdbReadCacheHashAttribute : TdbCacheBaseAttribute
    {
        /// <summary>
        /// key前缀
        /// </summary>
        private string KeyPrefix { get; set; }

        /// <summary>
        /// 参数位置（从0算起）
        /// </summary>
        public int ParamIndex { get; set; } = -1;

        /// <summary>
        /// 从指定属性获取
        /// </summary>
        public string FromPropertyName { get; set; } = "";

        /// <summary>
        /// 缓存天数（默认1天，即缓存会在第2天指定时间点失效）
        /// </summary>
        public int TimeoutDays { get; set; } = 1;

        /// <summary>
        /// 过期时间（格式：HH:mm:ss，默认值："01:00:00"，即缓存会在缓存天数+此时间点失效）
        /// </summary>
        public string ExpireAtTime { get; set; } = "01:00:00";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="keyPrefix">key前缀</param>
        public TdbReadCacheHashAttribute(string keyPrefix)
        {
            this.KeyPrefix = keyPrefix;
        }

        /// <summary>
        /// 获取hash key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        public string GetKey(IInvocation invocation)
        {
            var key = new StringBuilder(this.KeyPrefix);
            if (this.ParamIndex >= 0)
            {
                //获取参数值
                var param = invocation.GetArgumentValue(this.ParamIndex);
                //直接获取
                if (string.IsNullOrWhiteSpace(this.FromPropertyName))
                {
                    key.Append($".{param.ToStr()}");
                }
                //从属性获取
                else
                {
                    //属性不存在
                    if (CommHelper.IsExistPropertyOrField(param, this.FromPropertyName) == false)
                    {
                        throw new TdbException($"[缓存拦截器]找不到属性：{param.GetType().Name}.{this.FromPropertyName}");
                    }

                    var paramValue = CommHelper.ReflectGet(param, this.FromPropertyName);
                    key.Append($".{paramValue.ToStr()}");
                }
            }
            return key.ToString();
        }
    }
}
