using Microsoft.AspNetCore.Authorization;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 客户端IP要求与token中一致
    /// </summary>
    public class TdbClientIPRequirement : IAuthorizationRequirement
    {
    }
}
