using System;
using System.Collections.Generic;
using System.Text;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// 日志实例
    /// </summary>
    public class TdbLogger
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static ITdbLog? log;

        /// <summary>
        /// 日志实例
        /// </summary>
        public static ITdbLog Ins
        {
            get
            {
                if (log == null)
                {
                    throw new TdbException("未配置日志服务，请先在IServiceCollection添加日志服务");
                }

                return log;
            }
        }

        /// <summary>
        /// 初始化日志
        /// </summary>
        /// <param name="log">日志服务</param>
        internal static void InitLog(ITdbLog log)
        {
            TdbLogger.log = log;
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private TdbLogger()
        {
        }
    }
}
