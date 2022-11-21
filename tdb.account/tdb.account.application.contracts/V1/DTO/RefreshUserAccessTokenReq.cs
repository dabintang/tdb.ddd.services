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
    /// 刷新用户访问令牌 请求参数
    /// </summary>
    public class RefreshUserAccessTokenReq
    {
        /// <summary>
        /// [必填]刷新令牌
        /// </summary>
        [TdbRequired("刷新令牌")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// [必填]客户端IP（内部参数，webapi上看不到）
        /// </summary>
        [JsonIgnore]
        public string ClientIP { get; set; }
    }
}
