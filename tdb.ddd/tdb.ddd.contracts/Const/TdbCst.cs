using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 常量
    /// </summary>
    public class TdbCst
    {
        /// <summary>
        /// 服务ID常量
        /// </summary>
        public class ServerID
        {
            /// <summary>
            /// 账户服务
            /// </summary>
            public const int Account = 1;

            /// <summary>
            /// 文件服务
            /// </summary>
            public const int Files = 2;
			
            /// <summary>
            /// 运维服务
            /// </summary>
            public const int Admin = 3;

            /// <summary>
            /// 人际关系服务
            /// </summary>
            public const int Relationships = 11;
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
            // 账户服务     1001-1999
            // 文件服务     2001-2999
            // 运维服务     3001-3999

            // 人际关系服务 11001-11999
        }

        /// <summary>
        /// cap头部消息key
        /// </summary>
        public class CAPHeaders
        {
            /// <summary>
            /// 服务ID
            /// </summary>
            public const string ServerID = "tdb-cap-server-id";

            /// <summary>
            /// 服务序号
            /// </summary>
            public const string ServerSeq = "tdb-cap-server-seq";

            /// <summary>
            /// 触发CAP的事件描述
            /// </summary>
            public const string Source = "tdb-cap-source";
        }

        /// <summary>
        /// cap主题名称常量
        /// （注：主题名称一般由推送方定义，但对于一些支撑服务订阅提供的服务，统一定义）
        /// </summary>
        public class CAPTopic
        {
            /// <summary>
            /// 修改文件状态
            /// </summary>
            public const string UpdateFilesStatus = "UpdateFilesStatus";
        }
    }
}
