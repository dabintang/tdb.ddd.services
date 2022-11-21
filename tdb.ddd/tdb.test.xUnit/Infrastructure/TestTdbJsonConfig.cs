using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.infrastructure.Services;

namespace tdb.test.xUnit.Infrastructure
{
    /// <summary>
    /// 测试TdbJsonConfig类
    /// </summary>
    public class TestTdbJsonConfig
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TestTdbJsonConfig()
        {
        }

        /// <summary>
        /// 测试GetConfig
        /// </summary>
        [Fact]
        public void TestGetConfig()
        {
            var configService = new TdbJsonConfig(cfg =>
            {
                cfg.Path = "Configs\\jsonConfig.json";
                cfg.ReloadOnChange = true;
                cfg.Optional = false;
            });

            //获取配置信息
            var config = configService.GetConfig<JsonConfig>();

            //配置信息
            var configJson = "{\"AllowedHosts\":\"*\",\"URL\":\"http://*:11102\",\"DBConnStr\":\"server=127.0.0.1;database=tdb.password;user id=root;password=123456\",\"Token\":{\"Issuer\":\"tdb\",\"Audience\":\"tdb\",\"SecretKey\":\"0123456789223456\",\"TimeoutSeconds\":360000,\"Client\":{\"AppID\":\"1234567\"}},\"ConcurrentNum\":10}";

            Assert.Equal(config.SerializeJson(), configJson);
        }
    }

    /// <summary>
    /// 配置信息
    /// </summary>
    public class JsonConfig
    {
        /// <summary>
        /// 允许访问的主机
        /// </summary>
        [TdbConfigKey("AllowedHosts")]
        public string AllowedHosts { get; set; }

        /// <summary>
        /// 本服务URL
        /// </summary>
        [TdbConfigKey("Kestrel:EndPoints:Server:Url")]
        public string URL { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        [TdbConfigKey("DBConnStr")]
        public string DBConnStr { get; set; }

        /// <summary>
        /// token配置信息
        /// </summary>
        [TdbConfigKey("Token")]
        public TokenConfig Token { get; set; }

        /// <summary>
        /// 并发数
        /// </summary>
        [TdbConfigKey("ConcurrentNum")]
        public int ConcurrentNum { get; set; }

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
            /// 受众
            /// </summary>
            public string Audience { get; set; }

            /// <summary>
            /// 秘钥
            /// </summary>
            public string SecretKey { get; set; }

            /// <summary>
            /// 超时时间（秒）
            /// </summary>
            public int TimeoutSeconds { get; set; }

            /// <summary>
            /// client配置信息
            /// </summary>
            public ClientConfig Client { get; set; }
        }

        /// <summary>
        /// client配置信息
        /// </summary>
        public class ClientConfig
        {
            /// <summary>
            /// AppID
            /// </summary>
            public string AppID { get; set; }
        }
    }
}
