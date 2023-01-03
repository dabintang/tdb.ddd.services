﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.files.infrastructure.Config
{
    /// <summary>
    /// appsettings.json配置
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 服务配置信息
        /// </summary>
        [TdbConfigKey("Server")]
        public ServerConfig Server { get; set; }

        /// <summary>
        /// token配置信息
        /// </summary>
        [TdbConfigKey("Token")]
        public TokenConfig Token { get; set; }

        /// <summary>
        /// 服务配置信息
        /// </summary>
        [TdbConfigKey("DB")]
        public DBConfig DB { get; set; }

        /// <summary>
        /// redis连接字符串
        /// </summary>
        [TdbConfigKey("RedisConnStr")]
        public List<string> RedisConnStr { get; set; }

        /// <summary>
        /// 文件存储路径
        /// </summary>
        [TdbConfigKey("FilesPath")]
        public string FilesPath { get; set; }

        /// <summary>
        /// 服务配置信息
        /// </summary>
        public class ServerConfig
        {
            /// <summary>
            /// 服务ID（0-255）
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// 服务ID（0-127）
            /// </summary>
            public int Seq { get; set; }
        }

        /// <summary>
        /// token配置信息
        /// </summary>
        public class TokenConfig
        {
            /// <summary>
            /// 秘钥
            /// </summary>
            public string SecretKey { get; set; }
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        public class DBConfig
        {
            /// <summary>
            /// 数据库连接字符串
            /// </summary>
            public string ConnStr { get; set; }
        }
    }
}
