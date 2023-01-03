using tdb.ddd.infrastructure;

namespace tdb.ddd.account.tools.Configs
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class ToolsConfig
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public static void Init()
        {
            //读取appsettings.json配置
            ReadAppConfig();
            //配置有改动时重新读取
            TdbConfig.Ins.ConfigReload += RefreshAppConfig;
        }

        /// <summary>
        /// 读取appsettings.json配置
        /// </summary>
        private static void ReadAppConfig()
        {
            App = TdbConfig.Ins.GetConfig<AppConfig>();
        }

        /// <summary>
        /// appsettings.json配置有变动时更新
        /// </summary>
        private static void RefreshAppConfig()
        {
            //读取appsettings.json配置
            ReadAppConfig();
        }

        /// <summary>
        /// appsettings.json配置
        /// </summary>
        public static AppConfig App { get; private set; }
    }
}
