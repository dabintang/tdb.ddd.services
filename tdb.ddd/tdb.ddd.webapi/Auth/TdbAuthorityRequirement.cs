using Microsoft.AspNetCore.Authorization;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 权限要求
    /// </summary>
    public class TdbAuthorityRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public int AuthorityID { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authorityID">权限ID</param>
        public TdbAuthorityRequirement(int authorityID)
        {
            this.AuthorityID = authorityID;
        }
    }
}
