using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.admin.domain.contracts.ConsulConfig
{
    /// <summary>
    /// 账户服务配置信息
    /// </summary>
    public class AccountConfigInfo
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// key前缀
        /// </summary>
        public const string PrefixKey = "tdb.ddd.account.";

        /// <summary>
        /// token配置信息
        /// </summary>
        [TdbConfigKey("Token")]
        public TokenConfigInfo Token { get; set; }

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
        /// 初始化数据配置
        /// </summary>
        [TdbConfigKey("InitData")]
        public InitDataConfigInfo InitData { get; set; }

        #region 内部类

        /// <summary>
        /// token配置信息
        /// </summary>
        public class TokenConfigInfo
        {
            /// <summary>
            /// 访问令牌有效时间（秒）
            /// </summary>
            public int AccessTokenValidSeconds { get; set; }

            /// <summary>
            /// 刷新令牌有效时间（秒）
            /// </summary>
            public int RefreshTokenValidSeconds { get; set; }
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
        /// 初始化数据配置
        /// </summary>
        public class InitDataConfigInfo
        {
            /// <summary>
            /// 超级管理员登录名
            /// </summary>
            public string SuperAdminLoginName { get; set; }

            /// <summary>
            /// 超级管理员默认密码
            /// </summary>
            public string SuperAdminDefaultPwd { get; set; }

            /// <summary>
            /// 超级管理员手机号码
            /// </summary>
            public string SuperAdminMobilePhone { get; set; }

            /// <summary>
            /// 超级管理员电子邮箱
            /// </summary>
            public string SuperAdminEmail { get; set; }
        }

        #endregion

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}
