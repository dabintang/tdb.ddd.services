using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.Role;

namespace tdb.account.domain.BusMediatR
{
    /// <summary>
    /// 判断角色是否存在 请求参数
    /// </summary>
    public class IsRoleExistRequest : IRequest<bool>
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleID { get; set; }
    }

    /// <summary>
    /// 判断角色是否存在
    /// </summary>
    public class IsRoleExistRequestHandler : IRequestHandler<IsRoleExistRequest, bool>
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(IsRoleExistRequest request, CancellationToken cancellationToken)
        {
            var roleService = new RoleService();
            return await roleService.IsExist(request.RoleID);
        }
    }
}
