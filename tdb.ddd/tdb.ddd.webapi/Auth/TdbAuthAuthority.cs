using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using static tdb.ddd.contracts.TdbCst;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 权限要求特性
    /// </summary>
    public class TdbAuthAuthorityAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 策略名
        /// </summary>
        public const string PolicyName = "TdbAuthPolicy.Authority";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        public TdbAuthAuthorityAttribute(long authorityID) : base($"{PolicyName}{TdbAuthPolicyProvider.PolicyNameSeparator}{authorityID}")
        {
        }
    }

    /// <summary>
    /// 权限要求
    /// </summary>
    public class TdbAuthAuthorityRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public long AuthorityID { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        public TdbAuthAuthorityRequirement(long authorityID)
        {
            this.AuthorityID = authorityID;
        }
    }

    /// <summary>
    /// 权限要求处理
    /// </summary>
    public class TdbAuthAuthorityHandler : AuthorizationHandler<TdbAuthAuthorityRequirement>, ITdbIOCScoped
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TdbAuthAuthorityRequirement requirement)
        {
            //获取权限
            var lstAuthorityClaim = context.User.FindAll(TdbClaimTypes.AuthorityID);
            var lstAuthorityID = lstAuthorityClaim.SelectMany(m => m.Value.DeserializeJson<List<long>>() ?? new List<long>()).ToList();

            //判断权限
            if (lstAuthorityID.Contains(requirement.AuthorityID))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
