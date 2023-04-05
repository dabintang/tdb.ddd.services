using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.relationships.infrastructure.Config
{
    /// <summary>
    /// 分布式配置信息
    /// </summary>
    public class DistributedConfigInfo
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// key前缀
        /// </summary>
        public const string PrefixKey = "tdb.ddd.relationships.";

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

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}
