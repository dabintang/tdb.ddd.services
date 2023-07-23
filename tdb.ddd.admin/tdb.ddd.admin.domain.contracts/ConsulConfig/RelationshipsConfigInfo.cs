using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.admin.domain.contracts.ConsulConfig
{
    /// <summary>
    /// 人际关系服务配置信息
    /// </summary>
    public class RelationshipsConfigInfo
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

        /// <summary>
        /// AES加解密信息
        /// </summary>
        [TdbConfigKey("AES")]
        public AESConfigInfo AES { get; set; }

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

        /// <summary>
        /// AES加解密信息
        /// </summary>
        public class AESConfigInfo
        {
            /// <summary>
            /// 秘钥（支持长度：16、24、32）
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// 向量（支持长度：16）
            /// </summary>
            public string IV { get; set; }
        }

        #endregion

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}
