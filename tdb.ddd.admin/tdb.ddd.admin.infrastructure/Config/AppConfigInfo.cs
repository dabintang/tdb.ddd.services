﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.admin.infrastructure.Config
{
    /// <summary>
    /// appsettings.json配置
    /// </summary>
    public class AppConfigInfo
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// 服务配置信息
        /// </summary>
        [TdbConfigKey("Server")]
        public ServerConfigInfo Server { get; set; }

        /// <summary>
        /// consul配置信息
        /// </summary>
        [TdbConfigKey("Consul")]
        public AppConsulConfigInfo Consul { get; set; }

        /// <summary>
        /// token配置信息
        /// </summary>
        [TdbConfigKey("Token")]
        public TokenConfigInfo Token { get; set; }

        /// <summary>
        /// HashID配置信息
        /// </summary>
        [TdbConfigKey("HashID")]
        public HashIDConfigInfo HashID { get; set; }

        /// <summary>
        /// 白名单IP
        /// </summary>
        [TdbConfigKey("WhiteListIP")]
        public List<string> WhiteListIP { get; set; }

        /// <summary>
        /// CAP配置信息
        /// </summary>
        [TdbConfigKey("CAP")]
        public CAPConfigInfo CAP { get; set; }

        /// <summary>
        /// 数据库配置信息
        /// </summary>
        [TdbConfigKey("DB")]
        public DBConfigInfo DB { get; set; }

        #region 内部类

        /// <summary>
        /// 服务配置信息
        /// </summary>
        public class ServerConfigInfo
        {
            /// <summary>
            /// 服务ID（0-127）
            /// </summary>
            public int Seq { get; set; }

            /// <summary>
            /// 环境
            /// </summary>
            public string Environment { get; set; }
        }

        /// <summary>
        /// consul配置信息
        /// </summary>
        public class AppConsulConfigInfo
        {
            /// <summary>
            /// IP
            /// </summary>
            public string IP { get; set; }

            /// <summary>
            /// 端口
            /// </summary>
            public int Port { get; set; }
        }

        /// <summary>
        /// token配置信息
        /// </summary>
        public class TokenConfigInfo
        {
            /// <summary>
            /// 发行者
            /// </summary>
            public string Issuer { get; set; }

            /// <summary>
            /// 秘钥
            /// </summary>
            public string SecretKey { get; set; }
        }

        /// <summary>
        /// HashID配置信息
        /// </summary>
        public class HashIDConfigInfo
        {
            /// <summary>
            /// 盐
            /// </summary>
            public string Salt { get; set; }
        }

        /// <summary>
        /// CAP配置信息
        /// </summary>
        public class CAPConfigInfo
        {
            /// <summary>
            /// redis链接字符串(StackExchange.Redis)
            /// </summary>
            public string RedisConnStr { get; set; }

            /// <summary>
            /// 数据库链接字符串(mysql)
            /// </summary>
            public string DBConnStr { get; set; }
        }

        /// <summary>
        /// 数据库配置信息
        /// </summary>
        public class DBConfigInfo
        {
            /// <summary>
            /// 连接字符串
            /// </summary>
            public string ConnStr { get; set; }
        }

        #endregion

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}
