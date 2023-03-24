using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;

namespace tdb.ddd.account.domain.User.Aggregate
{
    /// <summary>
    /// 电子邮箱值对象
    /// </summary>
    public class EmailValueObject : TdbValueObject
    {
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// 电子邮箱是否已验证
        /// </summary>
        public bool IsEmailVerified { get; set; }
    }
}
