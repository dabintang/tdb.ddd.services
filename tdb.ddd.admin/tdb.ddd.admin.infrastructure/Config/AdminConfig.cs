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
    }
}
