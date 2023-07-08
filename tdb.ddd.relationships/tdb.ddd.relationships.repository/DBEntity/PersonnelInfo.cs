using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.relationships.repository.DBEntity
{
    /// <summary>
    /// 人员信息表
    /// </summary>
    [SugarTable("personnel_info")]
    public class PersonnelInfo
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Name { get; set; } = "";

        /// <summary>
        /// 性别（1：男；2：女；3：未知）
        /// </summary>
        public byte GenderCode { get; set; }

        /// <summary>
        /// 头像图片ID
        /// </summary>
        public long? HeadImgID { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [SugarColumn(Length = 16)]
        public string MobilePhone { get; set; } = "";

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [SugarColumn(Length = 128)]
        public string Email { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = 255)]
        public string Remark { get; set; } = "";

        /// <summary>
        /// 用户账户ID
        /// </summary>
        public long? UserID { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public long CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        public long UpdaterID { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
