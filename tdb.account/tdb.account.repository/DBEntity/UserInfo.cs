using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.contracts.Enum;
using tdb.account.domain.User.Aggregate;
using tdb.ddd.domain;

namespace tdb.account.repository.DBEntity
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    [SugarTable("user_info")]
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Name { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(Length = 32)]
        public string NickName { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        [SugarColumn(Length = 32)]
        public string LoginName { get; set; }

        /// <summary>
        /// 密码(MD5)
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Password { get; set; }

        /// <summary>
        /// 性别（1：男；2：女；3：未知）
        /// </summary>
        public byte GenderCode { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [SugarColumn(Length = 16)]
        public string MobilePhone { get; set; }

        /// <summary>
        /// 手机号是否已验证
        /// </summary>
        public bool IsMobilePhoneVerified { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [SugarColumn(Length = 128)]
        public string Email { get; set; }

        /// <summary>
        /// 电子邮箱是否已验证
        /// </summary>
        public bool IsEmailVerified { get; set; }

        /// <summary>
        /// 状态（1：激活；2：禁用）
        /// </summary>
        public byte StatusCode { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = 256)]
        public string Remark { get; set; }

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
