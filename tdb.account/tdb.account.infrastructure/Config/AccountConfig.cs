using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.infrastructure;

namespace tdb.account.infrastructure.Config
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public static class AccountConfig
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

        /// <summary>
        /// 回报消息配置
        /// </summary>
        public static MsgConfig Msg { get; private set; }
    }
}
