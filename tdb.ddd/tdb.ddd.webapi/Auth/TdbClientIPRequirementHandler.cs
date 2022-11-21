﻿using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 客户端IP要求与token中一致 处理
    /// </summary>
    public class TdbClientIPRequirementHandler : AuthorizationHandler<TdbClientIPRequirement>, ITdbClientIPRequirementHandler
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TdbClientIPRequirement requirement)
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
            var clientIP = filterContext?.HttpContext.GetClientIP();

            //判断IP是否一致
            if (clientIP == tokenIP)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail(new AuthorizationFailureReason(this, "客户端IP与token中的不一致。{this.GetUserInfo(context.User)}"));
            TdbLogger.Ins.Warn("客户端IP与token中的不一致");
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

    /// <summary>
    /// 
    /// </summary>
    public interface ITdbClientIPRequirementHandler : ITdbIOCScoped
    {
    }
}
