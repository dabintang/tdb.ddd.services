using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.infrastructure;

namespace tdb.ddd.admin.infrastructure.Config
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public static class AdminConfig
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public static void Init()
        {
            //json配置服务
            JsonConfigService = TdbConfigFactory.CreateAppsettingsConfig();
            //读取appsettings.json配置
            ReadAppConfig();
            //配置有改动时重新读取
            JsonConfigService.ConfigReload += RefreshAppConfig;

            //共用配置服务
            CommonConfigService = TdbConfigFactory.CreateConsulConfig(App.Consul.IP, App.Consul.Port, CommonConfigInfo.PrefixKey);
            //读取共用配置信息
            ReadCommonConfigInfo();
            //定时重新读取共用配置信息
            RefreshCommonConfigInfo();

        }

        #region appsetting.json

        /// <summary>
        /// appsettings.json配置
        /// </summary>
        public static AppConfigInfo App { get; private set; }

        /// <summary>
        /// json配置服务
        /// </summary>
        private static ITdbJsonConfig JsonConfigService { get; set; }

        /// <summary>
        /// 读取appsettings.json配置
        /// </summary>
        private static void ReadAppConfig()
        {
            App = JsonConfigService.GetConfig<AppConfigInfo>();
        }

        /// <summary>
        /// appsettings.json配置有变动时更新
        /// </summary>
        private static void RefreshAppConfig()
        {
            //读取appsettings.json配置
            ReadAppConfig();
        }

        #endregion

        #region common

        /// <summary>
        /// 共用配置
        /// </summary>
        public static CommonConfigInfo Common { get; private set; }

        /// <summary>
        /// 共用配置服务
        /// </summary>
        private static ITdbDistributedConfig CommonConfigService { get; set; }

        /// <summary>
        /// 读取共用配置信息
        /// </summary>
        private static void ReadCommonConfigInfo()
        {
            Common = CommonConfigService.GetConfigAsync<CommonConfigInfo>().Result;
        }

        /// <summary>
        /// 定时重新读取共用配置信息
        /// </summary>
        private static void RefreshCommonConfigInfo()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    //5分钟重新读取一次配置
                    Thread.Sleep(5 * 60 * 1000);
                    ReadCommonConfigInfo();
                }
            });
        }

        #endregion

    }
}
