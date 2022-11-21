using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.account.domain.BusMediatR
{
    /// <summary>
    /// 获取角色拥有的权限ID 请求参数
    /// </summary>
    public class GetRoleAuthorityIDRequest : IRequest<List<int>>
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleID { get; set; }
    }
}
