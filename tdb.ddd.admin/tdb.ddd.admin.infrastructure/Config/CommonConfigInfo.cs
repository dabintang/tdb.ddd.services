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

        #endregion
    }
}
