using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        /// 人际圈ID
        /// </summary>
        [JsonInclude]
        public long CircleID { get; internal set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        [JsonInclude]
        public long PersonnelID { get; internal set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [JsonInclude]
        public EnmRole RoleCode { get; internal set; }

        /// <summary>
        /// 圈内身份
        /// </summary>
        [JsonInclude]
        public string Identity { get; internal set; } = "";

        /// <summary>
        /// 创建信息
        /// </summary>
        [JsonInclude]
        public CreateInfoValueObject CreateInfo { get; internal set; } = new CreateInfoValueObject();

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
        /// <param name="circleID">人际圈ID</param>
        /// <param name="personnelID">人员ID</param>
        /// <param name="roleCode">角色编码</param>
        /// <param name="identity">身份</param>
        /// <param name="creatorID">创建人ID</param>
        /// <param name="createTime">创建时间</param>
        public MemberEntity(long circleID, long personnelID, EnmRole roleCode, string identity, long creatorID, DateTime createTime)
        {
            this.ID = RelationshipsUniqueIDHelper.CreateID();
            this.CircleID = circleID;
            this.PersonnelID = personnelID;
            this.RoleCode = roleCode;
            this.Identity = identity;
            this.CreateInfo.CreatorID = creatorID;
            this.CreateInfo.CreateTime = createTime;
        }
    }
}
