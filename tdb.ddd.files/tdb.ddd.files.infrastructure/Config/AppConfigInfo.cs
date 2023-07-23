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
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

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
            /// 服务序号（0-127）
            /// </summary>
            public int Seq { get; set; }

            /// <summary>
            /// 环境
            /// </summary>
            public string Environment { get; set; }
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

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}
