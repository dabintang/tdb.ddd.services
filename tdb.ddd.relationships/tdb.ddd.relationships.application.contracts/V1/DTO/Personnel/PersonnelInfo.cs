using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.relationships.domain.contracts.Enum;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Personnel
{
    /// <summary>
    /// 人员信息
    /// </summary>
    public class PersonnelInfo
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }

        /// <summary>
        /// 头像图片ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long? HeadImgID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 性别
        /// </summary>
        public EnmGender GenderCode { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; } = "";

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";
    }
}
