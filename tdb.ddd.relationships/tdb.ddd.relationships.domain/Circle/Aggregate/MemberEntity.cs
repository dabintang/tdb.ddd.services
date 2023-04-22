using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;
using tdb.ddd.relationships.domain.contracts.Enum;
using tdb.ddd.relationships.infrastructure;

namespace tdb.ddd.relationships.domain.Circle.Aggregate
{
    /// <summary>
    /// 成员信息实体
    /// </summary>
    public class MemberEntity : TdbEntity<long>
    {
        #region 值

        /// <summary>
        /// 人员ID
        /// </summary>
        public long PersonnelID { get; internal set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public EnmRole RoleCode { get; internal set; }

        /// <summary>
        /// 圈内身份
        /// </summary>
        public string Identity { get; internal set; } = "";

        #endregion

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public MemberEntity()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <param name="roleCode">角色编码</param>
        /// <param name="identity">身份</param>
        public MemberEntity(long personnelID, EnmRole roleCode, string identity = "")
        {
            this.ID = RelationshipsUniqueIDHelper.CreateID();
            this.PersonnelID = personnelID;
            this.RoleCode = roleCode;
            this.Identity = identity;
        }
    }
}
