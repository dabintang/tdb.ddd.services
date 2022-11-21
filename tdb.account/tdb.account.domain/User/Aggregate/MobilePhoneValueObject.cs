using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;

namespace tdb.account.domain.User.Aggregate
{
    /// <summary>
    /// 手机号码值对象
    /// </summary>
    public class MobilePhoneValueObject : TdbValueObject
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 手机号是否已验证
        /// </summary>
        public bool IsMobilePhoneVerified { get; set; }

    }
}
