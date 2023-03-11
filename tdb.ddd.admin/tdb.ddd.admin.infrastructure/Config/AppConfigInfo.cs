using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.admin.infrastructure.Config
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
        public AppConsulConfigInfo Consul { get; set; }

        /// <summary>
        /// token配置信息
        /// </summary>
        [TdbConfigKey("Token")]
        public TokenConfigInfo Token { get; set; }

        /// <summary>
        /// HashID配置信息
        /// </summary>
        [TdbConfigKey("HashID")]
        public HashIDConfigInfo HashID { get; set; }

        /// <summary>
        /// 白名单IP
        /// </summary>
        [TdbConfigKey("WhiteListIP")]
        public List<string> WhiteListIP { get; set; }

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
        public class AppConsulConfigInfo
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

        /// <summary>
        /// token配置信息
        /// </summary>
        public class TokenConfigInfo
        {
            /// <summary>
            /// 发行者
            /// </summary>
            public string Issuer { get; set; }

            /// <summary>
            /// 秘钥
            /// </summary>
            public string SecretKey { get; set; }
        }

        /// <summary>
        /// HashID配置信息
        /// </summary>
        public class HashIDConfigInfo
        {
            /// <summary>
            /// 盐
            /// </summary>
            public string Salt { get; set; }
        }

        #endregion
    }
}
