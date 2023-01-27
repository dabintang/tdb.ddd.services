using System;
using System.Collections.Generic;
using System.Text;
using tdb.consul.kv;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 配置Key特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TdbConfigKeyAttribute : Attribute, IConsulConfigAttribute
    {
        /// <summary>
        /// 配置key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 配置key特性
        /// </summary>
        /// <param name="key">配置key</param>
        public TdbConfigKeyAttribute(string key)
        {
            this.Key = key;
        }
    }
}
