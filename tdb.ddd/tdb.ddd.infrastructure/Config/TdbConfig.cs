using System;
using System.Collections.Generic;
using System.Text;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// 配置服务
    /// </summary>
    public class TdbConfig
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        private static ITdbConfig? config = null;

        /// <summary>
        /// 本地配置服务
        /// </summary>
        public static ITdbConfig Ins
        {
            get
            {
                if (config == null)
                {
                    throw new TdbException("未添加配置服务，请先在IServiceCollection添加配置服务");
                }

                return config;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">指定服务</param>
        internal static void InitConfig(ITdbConfig config)
        {
            TdbConfig.config = config;
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private TdbConfig()
        {
        }
    }
}
