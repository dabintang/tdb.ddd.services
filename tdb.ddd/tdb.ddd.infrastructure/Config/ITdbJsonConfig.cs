using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// json配置接口
    /// </summary>
    public interface ITdbJsonConfig
    {
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetConfig<T>() where T : class, new();

        /// <summary>
        /// 配置重新加载事件
        /// </summary>
        event DelegateConfigReload? ConfigReload;
    }
}
