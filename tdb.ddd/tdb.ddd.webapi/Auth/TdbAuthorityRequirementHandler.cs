using Microsoft.AspNetCore.Authorization;
using System.Linq;
using tdb.ddd.contracts;
using tdb.ddd.domain;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 权限要求处理
    /// </summary>
    public class TdbAuthorityRequirementHandler : AuthorizationHandler<TdbAuthorityRequirement>, ITdbIOCScoped
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TdbAuthorityRequirement requirement)
        {
            //获取权限
            var lstAuthorityClaim = context.User.FindAll(TdbClaimTypes.AuthorityID);
            var lstAuthorityID = lstAuthorityClaim.Select(m => Convert.ToInt32(m.Value)).ToList();

            //判断是否有权限
            if (lstAuthorityID.Contains(requirement.AuthorityID))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
