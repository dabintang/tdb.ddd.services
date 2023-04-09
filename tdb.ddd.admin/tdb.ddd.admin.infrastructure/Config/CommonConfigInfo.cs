using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.admin.infrastructure.Config
{
    /// <summary>
    /// 共用配置信息
    /// </summary>
    public class CommonConfigInfo
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// key前缀
        /// </summary>
        public const string PrefixKey = "tdb.ddd.common.";

        /// <summary>
        /// 账户服务配置信息
        /// </summary>
        [TdbConfigKey("AccountService")]
        public ServiceConfigInfo AccountService { get; set; }

        /// <summary>
        /// 文件服务配置信息
        /// </summary>
        [TdbConfigKey("FilesService")]
        public ServiceConfigInfo FilesService { get; set; }

        /// <summary>
        /// 运维服务配置信息
        /// </summary>
        [TdbConfigKey("AdminService")]
        public ServiceConfigInfo AdminService { get; set; }

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
        /// CAP配置信息
        /// </summary>
        [TdbConfigKey("CAP")]
        public CAPConfigInfo CAP { get; set; }

        #region 内部类

        /// <summary>
        /// 服务配置信息
        /// </summary>
        public class ServiceConfigInfo
        {
            /// <summary>
            /// webapi根地址
            /// </summary>
            public string WebapiRootURL { get; set; }
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

        #endregion

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}
