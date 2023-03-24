using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.account.infrastructure.Config
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
            //json配置服务
            JsonConfigService = TdbConfigFactory.CreateAppsettingsConfig();
            //读取appsettings.json配置
            ReadAppConfigInfo();
            //配置有改动时重新读取
            JsonConfigService.ConfigReload += RefreshAppConfigInfo;

            //共用配置服务
            CommonConfigService = TdbConfigFactory.CreateConsulConfig(App.Consul.IP, App.Consul.Port, CommonConfigInfo.PrefixKey);
            //读取共用配置信息
            ReadCommonConfigInfo();
            //定时重新读取共用配置信息
            RefreshCommonConfigInfo();

            //分布式配置
            DistributedConfigService = TdbConfigFactory.CreateConsulConfig(App.Consul.IP, App.Consul.Port, DistributedConfigInfo.PrefixKey);
            //读取分布式配置信息
            ReadDistributedConfigInfo();
            //定时重新读取分布式配置信息
            RefreshDistributedConfigInfo();

            //消息配置
            var msgFullFileName = CommHelper.GetFullFileName("message.json");
            var msgJsonStr = File.ReadAllText(msgFullFileName);
            var msg = msgJsonStr.DeserializeJson<MsgConfigInfo>() ?? throw new TdbException("未设置本地配置【message.json】");
            Msg = msg;

            //验证配置信息是否正确
            CheckConfig();
        }

        #region appsettings.json

        private static AppConfigInfo? _app;
        /// <summary>
        /// appsettings.json配置
        /// </summary>
        public static AppConfigInfo App
        {
            get
            {
                if (_app is null)
                {
                    throw new TdbException("未设置本地配置【appsettings.json】");
                }

                return _app;
            }
            private set => _app = value;
        }

        /// <summary>
        /// json配置服务
        /// </summary>
        private static ITdbJsonConfig? JsonConfigService { get; set; }

        /// <summary>
        /// 读取appsettings.json配置
        /// </summary>
        private static void ReadAppConfigInfo()
        {
            App = JsonConfigService!.GetConfig<AppConfigInfo>();
        }

        /// <summary>
        /// appsettings.json配置有变动时更新
        /// </summary>
        private static void RefreshAppConfigInfo()
        {
            //读取appsettings.json配置
            ReadAppConfigInfo();
        }

        #endregion

        #region common

        private static CommonConfigInfo? _common;
        /// <summary>
        /// 共用配置
        /// </summary>
        public static CommonConfigInfo Common
        {
            get
            {
                if (_common is null)
                {
                    throw new TdbException("未设置分布式配置【共用配置】");
                }

                return _common;
            }
            private set => _common = value;
        }

        /// <summary>
        /// 共用配置服务
        /// </summary>
        private static ITdbDistributedConfig? CommonConfigService { get; set; }

        /// <summary>
        /// 读取共用配置信息
        /// </summary>
        private static void ReadCommonConfigInfo()
        {
            Common = CommonConfigService!.GetConfigAsync<CommonConfigInfo>().Result;
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

        #region 分布式配置

        private static DistributedConfigInfo? _distributed;
        /// <summary>
        /// 分布式配置
        /// </summary>
        public static DistributedConfigInfo Distributed
        {
            get
            {
                if (_distributed is null)
                {
                    throw new TdbException("未设置分布式配置【账户服务配置】");
                }

                return _distributed;
            }
            private set => _distributed = value;
        }

        /// <summary>
        /// 分布式配置服务
        /// </summary>
        private static ITdbDistributedConfig? DistributedConfigService { get; set; }

        /// <summary>
        /// 读取分布式配置信息
        /// </summary>
        private static void ReadDistributedConfigInfo()
        {
            Distributed = DistributedConfigService!.GetConfigAsync<DistributedConfigInfo>().Result;
        }

        /// <summary>
        /// 定时重新读取分布式配置信息
        /// </summary>
        private static void RefreshDistributedConfigInfo()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    //5分钟重新读取一次配置
                    Thread.Sleep(5 * 60 * 1000);
                    ReadDistributedConfigInfo();
                }
            });
        }

        #endregion

        private static MsgConfigInfo? _msg;
        /// <summary>
        /// 回报消息配置
        /// </summary>
        public static MsgConfigInfo Msg
        {
            get
            {
                if (_msg is null)
                {
                    throw new TdbException("未设置本地配置【message.json】");
                }

                return _msg;
            }
            private set => _msg = value;
        }

        #region 验证

        /// <summary>
        /// 验证配置信息是否正确
        /// </summary>
        private static void CheckConfig()
        {
            //TODO
        }

        #endregion
    }
}
