using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.relationships.infrastructure
{
    /// <summary>
    /// 人际关系服务常量
    /// </summary>
    public class RelationshipsCst
    {
        /// <summary>
        /// 缓存key前缀
        /// </summary>
        public class CacheKey
        {
            /// <summary>
            /// hash形式缓存的人员信息
            /// </summary>
            public const string HashPersonnelByID = "HashPersonnelByID";

            /// <summary>
            /// hash形式缓存的人员信息
            /// </summary>
            public const string HashPersonnelByUserID = "HashPersonnelByUserID";

            /// <summary>
            /// hash形式缓存的人际圈信息
            /// </summary>
            public const string HashCircleByID = "HashCircleByID";

            /// <summary>
            /// hash形式缓存的人际圈成员信息
            /// </summary>
            public const string HashCircleMemberByCircleID_PersonnelID = "HashCircleMemberByCircleID_PersonnelID";
        }
    }
}
