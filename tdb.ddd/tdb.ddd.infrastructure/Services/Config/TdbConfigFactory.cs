using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 配置服务工厂
    /// </summary>
    public class TdbConfigFactory
    {
        /// <summary>
        /// 创建appsettings.json文件配置服务
        /// </summary>
        public static TdbJsonConfig CreateAppsettingsConfig()
        {
            return new TdbJsonConfig(cfg =>
            {
                cfg.Path = "appsettings.json";
                cfg.ReloadOnChange = true;
                cfg.Optional = false;
            });
        }

        /// <summary>
        /// 创建json文件配置服务
        /// </summary>
        /// <param name="configureSource">配置源</param>
        public static TdbJsonConfig CreateJsonConfig(Action<JsonConfigurationSource> configureSource)
        {
            return new TdbJsonConfig(configureSource);
        }

        /// <summary>
        /// 创建consul配置服务
        /// </summary>
        /// <param name="consulIP">consul服务IP</param>
        /// <param name="consulPort">consul服务端口</param>
        /// <param name="prefixKey">key前缀，一般用来区分不同服务</param>
        /// <returns></returns>
        public static TdbConsulConfig CreateConsulConfig(string consulIP, int consulPort, string prefixKey)
        {
            return new TdbConsulConfig(consulIP, consulPort, prefixKey);
        }
    }
}
