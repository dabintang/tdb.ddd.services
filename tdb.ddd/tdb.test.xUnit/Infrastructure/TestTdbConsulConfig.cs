using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.test.xUnit.Infrastructure
{
    /// <summary>
    /// 测试TdbConsulConfig类
    /// </summary>
    public class TestTdbConsulConfig
    {
        /// <summary>
        /// 测试
        /// </summary>
        [Fact]
        public void Test()
        {
            //json配置服务
            var jsonConfigService = TdbConfigFactory.CreateJsonConfig(cfg =>
            {
                cfg.Path = "Configs\\consulConfig.json";
                cfg.ReloadOnChange = true;
                cfg.Optional = false;
            });

            //获取json配置信息
            var config = jsonConfigService.GetConfig<ConfigInfo>();
            Assert.Equal("127.0.0.1", config.Consul.IP);
            Assert.Equal(8500, config.Consul.Port);

            //consul配置服务
            var consulConfigService = TdbConfigFactory.CreateConsulConfig(config.Consul.IP, config.Consul.Port, "tdb.ddd.test.");
            //还原配置
            consulConfigService.RestoreConfigAsync(config).Wait();
            //读取配置
            var configConsul = consulConfigService.GetConfigAsync<ConfigInfo>().Result;
            Assert.Equal(configConsul.Consul.IP, config.Consul.IP);
            Assert.Equal(configConsul.Consul.Port, config.Consul.Port);
        }

        /// <summary>
        /// 配置信息
        /// </summary>
        public class ConfigInfo
        {
            /// <summary>
            /// Consul配置信息
            /// </summary>
            [TdbConfigKey("Consul")]
            public ConsulConfigInfo Consul { get; set; }


            /// <summary>
            /// Consul配置信息
            /// </summary>
            public class ConsulConfigInfo
            {
                /// <summary>
                /// consul IP
                /// </summary>
                public string IP { get; set; }

                /// <summary>
                /// consul端口号
                /// </summary>
                public int Port { get; set; }
            }
        }
    }
}
