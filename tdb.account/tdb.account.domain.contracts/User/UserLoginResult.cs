using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.account.domain.contracts.User
{
    /// <summary>
    /// 用户登录结果
    /// </summary>
    public class UserLoginResult
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
