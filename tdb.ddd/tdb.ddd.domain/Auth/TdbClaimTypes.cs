using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.domain
{
    /// <summary>
    /// 因为觉得ClaimTypes里的常量太长了，这里自定义一个短些的
    /// </summary>
    public class TdbClaimTypes
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public const string UID = "tdb_uid";

        /// <summary>
        /// 用户名
        /// </summary>
        public const string UName = "tdb_uname";

        /// <summary>
        /// 角色ID
        /// </summary>
        public const string RoleID = "tdb_role_id";

        /// <summary>
        /// 权限ID
        /// </summary>
        public const string AuthorityID = "tdb_authority_id";

        /// <summary>
        /// 客户端IP
        /// </summary>
        public const string ClientIP = "tdb_client_ip";
    }
}
