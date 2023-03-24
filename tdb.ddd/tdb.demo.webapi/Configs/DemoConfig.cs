using tdb.common;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.demo.webapi.Configs
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public static class DemoConfig
    {
        private static ITdbJsonConfig? JsonConfigService { get; set; }

        /// <summary>
        /// 初始化配置
        /// </summary>
        public static void Init()
        {
            JsonConfigService = TdbConfigFactory.CreateAppsettingsConfig();

            //读取appsettings.json配置
            ReadAppConfig();
            //配置有改动时重新读取
            JsonConfigService.ConfigReload += RefreshAppConfig;

            //消息配置
            var msgFullFileName = CommHelper.GetFullFileName("message.json");
            var msgJsonStr = File.ReadAllText(msgFullFileName);
            Msg = msgJsonStr.DeserializeJson<MsgConfig>();
        }

        /// <summary>
        /// 读取appsettings.json配置
        /// </summary>
        private static void ReadAppConfig()
        {
            App = JsonConfigService!.GetConfig<AppConfig>();
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
        public static AppConfig? App { get; private set; }

        /// <summary>
        /// 回报消息配置
        /// </summary>
        public static MsgConfig? Msg { get; private set; }
    }
}
