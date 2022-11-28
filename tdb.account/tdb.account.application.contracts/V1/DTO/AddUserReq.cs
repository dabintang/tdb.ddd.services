using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.contracts.Enum;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.account.application.contracts.V1.DTO
{
    /// <summary>
    /// 添加用户 请求参数
    /// </summary>
    public class AddUserReq
    {
        /// <summary>
        /// [选填]姓名（无值时会随机生成）
        /// </summary>
        [TdbStringLength("姓名", 32)]
        public string Name { get; set; }

        /// <summary>
        /// [选填]昵称（无值时会随机生成）
        /// </summary>
        [TdbStringLength("昵称", 32)]
        public string NickName { get; set; }

        /// <summary>
        /// [必填]登录名（唯一）
        /// </summary>
        [TdbRequired("登录名")]
        [TdbStringLength("登录名", 32)]
        public string LoginName { get; set; }

        /// <summary>
        /// 密码(MD5)
        /// </summary>
        [TdbRequired("密码")]
        [TdbStringLength("密码", 32)]
        public string Password { get; set; }

        /// <summary>
        /// [必填]性别（1：男；2：女；3：未知）
        /// </summary>
        [TdbRequired("性别")]
        [TdbEnumDataType(typeof(EnmGender), "性别不合法")]
        public EnmGender GenderCode { get; set; }

        /// <summary>
        /// [选填]生日
        /// </summary>
        public DateTime? Birthday { get; set; }
        
        /// <summary>
        /// [选填]手机号码（认证后唯一）
        /// </summary>
        [TdbStringLength("手机号码", 16)]
        [TdbRegex(TdbCstRegex.MobilePhone, "手机号码不正确")]
        public string MobilePhone { get; set; }

        /// <summary>
        /// [选填]电子邮箱
        /// </summary>
        [TdbStringLength("电子邮箱", 128)]
        [TdbRegex(TdbCstRegex.Email, "电子邮箱不正确")]
        public string Email { get; set; }

        /// <summary>
        /// [选填]备注
        /// </summary>
        [TdbStringLength("备注", 256)]
        public string Remark { get; set; }

        /// <summary>
        /// [选填]角色ID（只能赋予操作人拥有的角色[超级管理员除外]）
        /// </summary>
        public List<int> LstRoleID { get; set; }
    }
}
