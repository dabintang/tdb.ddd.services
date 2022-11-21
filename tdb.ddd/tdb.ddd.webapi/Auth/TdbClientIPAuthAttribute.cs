using Microsoft.AspNetCore.Authorization;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 客户端IP要求与token中一致
    /// </summary>
    public class TdbClientIPAuthAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 策略名
        /// </summary>
        public const string PolicyName = "PolicyClientIP";

        /// <summary>
        /// 构造函数
        /// </summary>
        public TdbClientIPAuthAttribute() : base(PolicyName)
        {
        }
    }
}
