using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.account.application.contracts.V1.DTO
{
    /// <summary>
    /// 用户登录 参数
    /// </summary>
    public class UserLoginReq
    {
        /// <summary>
        /// [必填]登录名
        /// </summary>
        [TdbRequired("登录名")]
        public string LoginName { get; set; }

        /// <summary>
        /// [必填]密码
        /// </summary>
        [TdbRequired("密码")]
        public string Password { get; set; }

        /// <summary>
        /// [必填]客户端IP（内部参数，webapi上看不到）
        /// </summary>
        [JsonIgnore]
        public string ClientIP { get; set; }
    }
}
