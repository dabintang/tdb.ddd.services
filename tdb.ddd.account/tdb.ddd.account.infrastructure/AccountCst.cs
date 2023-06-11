using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.account.infrastructure
{
    /// <summary>
    /// 账户服务常量
    /// </summary>
    public class AccountCst
    {
        /// <summary>
        /// 缓存key前缀
        /// </summary>
        public class CacheKey
        {
            /// <summary>
            /// hash形式缓存的用户信息
            /// </summary>
            public const string HashUserByID = "HashUserByID";

            /// <summary>
            /// hash形式缓存的用户角色ID
            /// </summary>
            public const string HashUserRoleIDsByUserID = "HashUserRoleIDsByUserID";

            /// <summary>
            /// hash形式缓存的角色信息
            /// </summary>
            public const string HashRoleByID = "HashRoleByID";

            /// <summary>
            /// hash形式缓存的角色权限ID
            /// </summary>
            public const string HashRoleAuthorityIDsByRoleID = "HashRoleAuthorityIDsByRoleID";

            /// <summary>
            /// hash形式缓存的权限信息
            /// </summary>
            public const string HashAuthorityByID = "HashAuthorityByID";

        }
    }
}
