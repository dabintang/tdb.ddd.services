using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 正则表达式常量
    /// </summary>
    public class TdbCstRegex
    {
        /// <summary>
        /// 座机号码
        /// </summary>
        public const string Telephone = @"^(\d{3,4}-)?\d{6,8}$";

        /// <summary>
        /// 手机号码
        /// </summary>
        public const string MobilePhone = @"^1[3456789]\d{9}$";

        /// <summary>
        /// 身份证号码
        /// </summary>
        public const string IDCard = @"(^\d{18}$)|(^\d{15}$)";

        /// <summary>
        /// 数字
        /// </summary>
        public const string Number = @"^[0-9]*$";

        /// <summary>
        /// 邮编
        /// </summary>
        public const string PostalCode = @"^\d{6}$";

        /// <summary>
        /// 邮箱
        /// </summary>
        public const string Email = @"^(\w)+(\.\w)*@(\w)+((\.\w+)+)$";
    }
}
