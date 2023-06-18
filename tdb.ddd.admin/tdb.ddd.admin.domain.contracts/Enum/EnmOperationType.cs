using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.admin.domain.contracts.Enum
{
    /// <summary>
    /// 操作类型（1：用户登录；2：初始化账户服务数据；3：还原共用配置；4：还原账户服务配置；5：还原文件服务配置；6：还原人际关系服务配置）
    /// </summary>
    public enum EnmOperationType : short
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        UserLogin = 1,

        /// <summary>
        /// 初始化账户服务数据
        /// </summary>
        InitAccountData = 2,

        /// <summary>
        /// 还原共用配置
        /// </summary>
        RestoreCommonConfig = 3,

        /// <summary>
        /// 还原账户服务配置
        /// </summary>
        RestoreAccountConfig = 4,

        /// <summary>
        /// 还原文件服务配置
        /// </summary>
        RestoreFilesConfig = 5,

        /// <summary>
        /// 还原运维服务配置
        /// </summary>
        RestoreAdminConfig = 6,

        /// <summary>
        /// 还原人际关系服务配置
        /// </summary>
        RestoreRelationshipsConfig = 7
    }
}
