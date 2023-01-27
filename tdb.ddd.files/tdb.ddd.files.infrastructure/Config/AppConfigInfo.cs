using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.files.infrastructure.Config
{
    /// <summary>
    /// appsettings.json配置
    /// </summary>
    public class AppConfigInfo
    {
        /// <summary>
        /// 服务配置信息
        /// </summary>
        [TdbConfigKey("Server")]
        public ServerConfigInfo Server { get; set; }

        /// <summary>
        /// consul配置信息
        /// </summary>
        [TdbConfigKey("Consul")]
        public ConsulConfigInfo Consul { get; set; }

        #region 内部类

        /// <summary>
        /// 服务配置信息
        /// </summary>
        public class ServerConfigInfo
        {
            /// <summary>
            /// 服务ID（0-127）
            /// </summary>
            public int Seq { get; set; }
        }

        /// <summary>
        /// consul配置信息
        /// </summary>
        public class ConsulConfigInfo
        {
            /// <summary>
            /// IP
            /// </summary>
            public string IP { get; set; }

            /// <summary>
            /// 端口
            /// </summary>
            public int Port { get; set; }
        }

        #endregion
    }
}
