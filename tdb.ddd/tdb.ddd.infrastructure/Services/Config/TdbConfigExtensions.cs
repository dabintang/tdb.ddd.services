using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 配置扩展
    /// </summary>
    public static class TdbConfigExtensions
    {
        /// <summary>
        /// 使用appsettings.json文件配置
        /// </summary>
        /// <param name="services"></param>
        public static void AddTdbAppsettingsConfig(this IServiceCollection services)
        {
            AddTdbConfig(services, () => new TdbJsonConfig(cfg =>
            {
                cfg.Path = "appsettings.json";
                cfg.ReloadOnChange = true;
                cfg.Optional = false;
            }));
        }

        /// <summary>
        /// 使用json文件配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureSource">配置源</param>
        public static void AddTdbJsonConfig(this IServiceCollection services, Action<JsonConfigurationSource> configureSource)
        {
            AddTdbConfig(services, () => new TdbJsonConfig(configureSource));
        }

        /// <summary>
        /// 添加指定服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="getTdbConfig">获取服务</param>
        public static void AddTdbConfig(this IServiceCollection services, Func<ITdbConfig> getTdbConfig)
        {
            var config = getTdbConfig();
            TdbConfig.InitConfig(config);

            services.AddSingleton(typeof(ITdbConfig), config);
        }
    }
}
