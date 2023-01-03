using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.ddd.account.application.contracts.V1.DTO
{
    /// <summary>
    /// 用户登录 参数
    /// </summary>
    public class UserLoginDTO
    {
        /// <summary>
        /// [必须]登录名
        /// </summary>
        [TdbRequired("登录名")]
        public string LoginName { get; set; }

        /// <summary>
        /// [必须]密码
        /// </summary>
        [TdbRequired("密码")]
        public string Password { get; set; }

        /// <summary>
        /// [必须]客户端IP（内部参数，webapi上看不到）
        /// </summary>
        [JsonIgnore]
        public string ClientIP { get; set; }
    }

    /// <summary>
    /// 刷新用户访问令牌 请求参数
    /// </summary>
    public class RefreshUserAccessTokenReq
    {
        /// <summary>
        /// [必须]刷新令牌
        /// </summary>
        [TdbRequired("刷新令牌")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// [必须]客户端IP（内部参数，webapi上看不到）
        /// </summary>
        [JsonIgnore]
        public string ClientIP { get; set; }
    }

    /// <summary>
    /// 用户登录结果
    /// </summary>
    public class UserLoginRes
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 访问令牌有效时间（秒）
        /// </summary>
        public int AccessTokenValidSeconds { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 刷新令牌有效时间（秒）
        /// </summary>
        public int RefreshTokenValidSeconds { get; set; }
    }
}
