using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.account.application.contracts.V1.DTO
{
    /// <summary>
    /// 更新用户密码 请求条件
    /// </summary>
    public class UpdateUserPasswordReq
    {
        /// <summary>
        /// [必须]用户ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("用户ID")]
        public long ID { get; set; }

        /// <summary>
        /// [必须]原密码
        /// </summary>
        [TdbRequired("原密码")]
        public string OldPassword { get; set; } = "";

        /// <summary>
        /// [必须]新密码
        /// </summary>
        [TdbRequired("新密码")]
        [TdbStringLength("密码", 32)]
        public string NewPassword { get; set; } = "";
    }
}
