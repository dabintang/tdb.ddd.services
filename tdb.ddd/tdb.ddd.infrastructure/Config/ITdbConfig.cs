using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// 配置服务
    /// </summary>
    public interface ITdbConfig
    {
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetConfig<T>() where T : class, new();

        /// <summary>
        /// 配置重新加载委托
        /// </summary>
        delegate void _ConfigReload();

        /// <summary>
        /// 配置重新加载事件
        /// </summary>
        event _ConfigReload? ConfigReload;
    }
}
