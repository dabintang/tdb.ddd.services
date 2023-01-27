using Microsoft.AspNetCore.Authorization;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 要求接口调用方IP为白名单IP 特性
    /// </summary>
    public class TdbAuthWhiteListIPAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 策略名
        /// </summary>
        public const string PolicyName = "TdbAuthPolicy.WhiteListIP";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strWhiteListIP">白名单IP，如有多个用逗号,隔开（如：127.0.0.1,localhost）；值为空时用默认白名单IP</param>
        public TdbAuthWhiteListIPAttribute(string strWhiteListIP = "")
        {
            if (string.IsNullOrWhiteSpace(strWhiteListIP))
            {
                this.Policy = PolicyName;
                return;
            }

            this.Policy = $"{PolicyName}{TdbAuthPolicyProvider.PolicyNameSeparator}{strWhiteListIP}";
        }
    }

    /// <summary>
    /// 要求接口调用方IP为白名单IP
    /// </summary>
    public class TdbAuthWhiteListIPRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 白名单IP集合
        /// </summary>
        public List<string> WhiteListIP { get; } = new List<string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="whiteListIP">白名单IP集合</param>
        public TdbAuthWhiteListIPRequirement(IEnumerable<string> whiteListIP)
        {
            this.WhiteListIP.AddRange(whiteListIP);
        }
    }

    /// <summary>
    /// 要求接口调用方IP为白名单IP 处理
    /// </summary>
    public class TdbAuthWhiteListIPHandler : AuthorizationHandler<TdbAuthWhiteListIPRequirement>, ITdbIOCScoped
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TdbAuthWhiteListIPRequirement requirement)
        {
            //获取客户端IP
            var filterContext = context.Resource as DefaultHttpContext;
            var clientIP = filterContext?.HttpContext.GetClientIP() ?? "";

            //判断IP是否白名单
            if (requirement.WhiteListIP.Contains(clientIP))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail(new AuthorizationFailureReason(this, $"客户端IP[{clientIP}]并非白名单[{requirement.WhiteListIP}]"));
            TdbLogger.Ins.Warn($"客户端IP[{clientIP}]并非白名单[{requirement.WhiteListIP.SerializeJson()}]");
            return Task.CompletedTask;
        }
    }
}
