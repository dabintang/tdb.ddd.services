using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.relationships.repository.DBEntity
{
    /// <summary>
    /// 人际圈成员信息表
    /// </summary>
    [SugarTable("circle_member_info")]
    public class CircleMemberInfo
    {
        /// <summary>
        /// 人际圈成员ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 人际圈ID
        /// </summary>
        public long CircleID { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        public long PersonnelID { get; set; }

        /// <summary>
        /// 角色编码(1：普通成员；2：管理员)
        /// </summary>
        public byte RoleCode { get; set; }

        /// <summary>
        /// 圈内身份
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Identity { get; set; } = "";

        /// <summary>
        /// 创建者ID
        /// </summary>
        public long CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
