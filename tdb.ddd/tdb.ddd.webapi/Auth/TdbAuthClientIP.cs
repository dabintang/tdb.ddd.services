using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 客户端IP要求与token中一致
    /// </summary>
    public class TdbAuthClientIPAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 策略名
        /// </summary>
        public const string PolicyName = "TdbAuthPolicy.ClientIP";

        /// <summary>
        /// 构造函数
        /// </summary>
        public TdbAuthClientIPAttribute() : base(PolicyName)
        {
        }
    }

    /// <summary>
    /// 客户端IP要求与token中一致
    /// </summary>
    public class TdbAuthClientIPRequirement : IAuthorizationRequirement
    {
    }

    /// <summary>
    /// 客户端IP要求与token中一致 处理
    /// </summary>
    public class TdbAuthClientIPHandler : AuthorizationHandler<TdbAuthClientIPRequirement>, ITdbIOCScoped
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TdbAuthClientIPRequirement requirement)
        {
            //获取token内IP
            var lstClientIPClaim = context.User.FindAll(TdbClaimTypes.ClientIP);
            var tokenIP = lstClientIPClaim.FirstOrDefault()?.Value;
            if (string.IsNullOrWhiteSpace(tokenIP))
            {
                context.Fail(new AuthorizationFailureReason(this, $"口令中未包含客户端IP。{GetUserInfo(context.User)}"));
                TdbLogger.Ins.Warn("口令中未包含客户端IP");
                return Task.CompletedTask;
            }

            //获取客户端IP
            var filterContext = context.Resource as DefaultHttpContext;
            var clientIP = filterContext?.HttpContext.GetClientIP() ?? "";

            //如果是本地ip（服务器间调用），直接通过
            if ((new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" }).Contains(clientIP))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            //判断IP是否一致
            if (clientIP == tokenIP)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail(new AuthorizationFailureReason(this, $"客户端IP与token中的不一致。{GetUserInfo(context.User)}"));
            TdbLogger.Ins.Warn($"客户端IP[{clientIP}]与token[{tokenIP}]中的不一致");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取用户描述信息
        /// </summary>
        /// <param name="user">用户身份信息</param>
        /// <returns></returns>
        private static string GetUserInfo(ClaimsPrincipal user)
        {
            if (user == null)
            {
                return "";
            }

            return $"[UID={user.FindFirst(TdbClaimTypes.UID)?.Value ?? ""}；UName={user.Identity?.Name ?? ""}]";
        }
    }
}
