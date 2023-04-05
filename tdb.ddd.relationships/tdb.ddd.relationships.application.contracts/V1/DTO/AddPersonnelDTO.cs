using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.relationships.domain.contracts.Enum;

namespace tdb.ddd.relationships.application.contracts.V1.DTO
{
    /// <summary>
    /// 添加人员参数
    /// </summary>
    public class AddPersonnelReq
    {
        /// <summary>
        /// [必须]姓名
        /// </summary>
        [TdbRequired("姓名")]
        [TdbStringLength("姓名", 32)]
        public string Name { get; set; } = "";

        /// <summary>
        /// 性别
        /// </summary>
        [TdbRequired("性别")]
        [TdbEnumDataType(typeof(EnmGender), "性别不合法")]
        public EnmGender GenderCode { get; set; }

        /// <summary>
        /// [可选]生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// [可选]手机号码
        /// </summary>
        [TdbStringLength("手机号码", 16)]
        [TdbRegex(TdbCstRegex.MobilePhone, "手机号码不正确")]
        public string? MobilePhone { get; set; }

        /// <summary>
        /// [可选]电子邮箱
        /// </summary>
        [TdbStringLength("电子邮箱", 128)]
        [TdbRegex(TdbCstRegex.Email, "电子邮箱不正确")]
        public string? Email { get; set; }

        /// <summary>
        /// [可选]备注
        /// </summary>
        [TdbStringLength("备注", 255)]
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 添加人员结果
    /// </summary>
    public class AddPersonnelRes
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }
    }
}
