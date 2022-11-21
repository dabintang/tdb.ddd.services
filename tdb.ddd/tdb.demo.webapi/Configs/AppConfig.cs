using tdb.ddd.infrastructure.Services;

namespace tdb.demo.webapi.Configs
{
    /// <summary>
    /// appsettings.json配置
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 本服务URL
        /// </summary>
        [TdbConfigKey("Kestrel:EndPoints:Server:Url")]
        public string URL { get; set; }

        /// <summary>
        /// 服务配置信息
        /// </summary>
        [TdbConfigKey("Server")]
        public ServerConfig Server { get; set; }

        /// <summary>
        /// token配置信息
        /// </summary>
        [TdbConfigKey("Token")]
        public TokenConfig Token { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        [TdbConfigKey("DBConnStr")]
        public string DBConnStr { get; set; }

        /// <summary>
        /// 服务配置信息
        /// </summary>
        public class ServerConfig
        {
            /// <summary>
            /// 服务ID（0-255）
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// 服务ID（0-127）
            /// </summary>
            public int Seq { get; set; }
        }

        /// <summary>
        /// token配置信息
        /// </summary>
        public class TokenConfig
        {
            /// <summary>
            /// 发行者
            /// </summary>
            public string Issuer { get; set; }

            /// <summary>
            /// 秘钥
            /// </summary>
            public string SecretKey { get; set; }

            /// <summary>
            /// 超时时间（秒）
            /// </summary>
            public int TimeoutSeconds { get; set; }
        }
    }
}
