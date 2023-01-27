using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.domain;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 角色要求特性
    /// </summary>
    public class TdbAuthRoleAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 策略名
        /// </summary>
        public const string PolicyName = "TdbAuthPolicy.Role";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="roleID">角色ID</param>
        public TdbAuthRoleAttribute(long roleID) : base($"{PolicyName}{TdbAuthPolicyProvider.PolicyNameSeparator}{roleID}")
        {
        }
    }

    /// <summary>
    /// 角色要求
    /// </summary>
    public class TdbAuthRoleRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleID { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="roleID">角色ID</param>
        public TdbAuthRoleRequirement(long roleID)
        {
            this.RoleID = roleID;
        }
    }

    /// <summary>
    /// 角色要求处理
    /// </summary>
    public class TdbAuthRoleHandler : AuthorizationHandler<TdbAuthRoleRequirement>, ITdbIOCScoped
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TdbAuthRoleRequirement requirement)
        {
            //获取用户拥有的角色
            var lstRoleClaim = context.User.FindAll(TdbClaimTypes.RoleID);
            var lstRoleID = lstRoleClaim.SelectMany(m => m.Value.DeserializeJson<List<long>>() ?? new List<long>()).ToList();

            //判断角色
            if (lstRoleID.Contains(requirement.RoleID))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
