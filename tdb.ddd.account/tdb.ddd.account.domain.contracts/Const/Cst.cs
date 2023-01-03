using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.account.domain.contracts.Const
{
    /// <summary>
    /// 常量
    /// </summary>
    public class Cst
    {
        /// <summary>
        /// 服务常量
        /// </summary>
        public class Server
        {
            /// <summary>
            /// 账户服务
            /// </summary>
            public const int Account = 1;

            /// <summary>
            /// 文件服务
            /// </summary>
            public const int Files = 2;
        }

        /// <summary>
        /// 用户ID常量
        /// </summary>
        public class UserID
        {
            #region 通用 1-99

            /// <summary>
            /// 超级管理员
            /// </summary>
            public const int SuperAdmin = 1;

            #endregion
        }

        /// <summary>
        /// 角色ID常量
        /// </summary>
        public class RoleID
        {
            #region 通用 1-99

            /// <summary>
            /// 超级管理员
            /// </summary>
            public const int SuperAdmin = 1;

            #endregion

            #region 账户服务 101-199

            /// <summary>
            /// 账户服务管理员
            /// </summary>
            public const int AccountAdmin = 101;

            #endregion
        }

        /// <summary>
        /// 权限ID常量
        /// </summary>
        public class AuthorityID
        {
            #region 通用 1-99

            #endregion

            #region 账户服务 101-199

            /// <summary>
            /// 用户增删改权限
            /// </summary>
            public const int AccountUserManage = 101;

            #endregion
        }

        /// <summary>
        /// 回报消息编码
        /// </summary>
        public class MsgCode
        {
            // 账户服务 10001-19999
            // 文件服务 20001-29999
        }
    }
}
