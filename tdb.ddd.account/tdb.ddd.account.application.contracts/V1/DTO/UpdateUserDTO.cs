using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.account.application.contracts.V1.DTO
{
    /// <summary>
    /// 更新用户 请求参数
    /// </summary>
    public class UpdateUserReq
    {
        /// <summary>
        /// [必须]用户ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("用户ID")]
        public long ID { get; set; }

        /// <summary>
        /// [可选]姓名（无值时会随机生成）
        /// </summary>
        [TdbStringLength("姓名", 32)]
        public string Name { get; set; }

        /// <summary>
        /// [可选]昵称（无值时会随机生成）
        /// </summary>
        [TdbStringLength("昵称", 32)]
        public string NickName { get; set; }

        /// <summary>
        /// [必须]性别（1：男；2：女；3：未知）
        /// </summary>
        [TdbRequired("性别")]
        [TdbEnumDataType(typeof(EnmGender), "性别不合法")]
        public EnmGender GenderCode { get; set; }

        /// <summary>
        /// [可选]生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// [可选]手机号码（认证后唯一）
        /// </summary>
        [TdbStringLength("手机号码", 16)]
        [TdbRegex(TdbCstRegex.MobilePhone, "手机号码不正确")]
        public string MobilePhone { get; set; }

        /// <summary>
        /// [可选]电子邮箱
        /// </summary>
        [TdbStringLength("电子邮箱", 128)]
        [TdbRegex(TdbCstRegex.Email, "电子邮箱不正确")]
        public string Email { get; set; }

        /// <summary>
        /// [可选]备注
        /// </summary>
        [TdbStringLength("备注", 255)]
        public string Remark { get; set; }
    }
}
