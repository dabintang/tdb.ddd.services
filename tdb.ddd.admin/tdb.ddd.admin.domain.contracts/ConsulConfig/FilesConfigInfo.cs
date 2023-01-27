using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.admin.domain.contracts.ConsulConfig
{
    /// <summary>
    /// 文件服务配置信息
    /// </summary>
    public class FilesConfigInfo
    {
        /// <summary>
        /// key前缀
        /// </summary>
        public const string PrefixKey = "tdb.ddd.files.";

        /// <summary>
        /// 本地文件夹路径
        /// </summary>
        [TdbConfigKey("FilesPath")]
        public string FilesPath { get; set; }

        /// <summary>
        /// 数据库配置信息
        /// </summary>
        [TdbConfigKey("DB")]
        public DBConfigInfo DB { get; set; }

        /// <summary>
        /// redis配置信息
        /// </summary>
        [TdbConfigKey("Redis")]
        public RedisConfigInfo Redis { get; set; }

        #region 内部类

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

        /// <summary>
        /// redis配置信息
        /// </summary>
        public class RedisConfigInfo
        {
            /// <summary>
            /// 连接字符串
            /// </summary>
            public List<string> ConnStr { get; set; }
        }

        #endregion
    }
}
