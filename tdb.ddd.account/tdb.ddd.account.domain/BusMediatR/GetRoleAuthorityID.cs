using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Role;

namespace tdb.ddd.account.domain.BusMediatR
{
    /// <summary>
    /// 获取角色拥有的权限ID 请求参数
    /// </summary>
    public class GetRoleAuthorityIDRequest : IRequest<List<long>>
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleID { get; set; }
    }

    /// <summary>
    /// 获取角色拥有的权限ID
    /// </summary>
    public class GetRoleAuthorityIDRequestHandler : IRequestHandler<GetRoleAuthorityIDRequest, List<long>>
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="request">请求参数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<long>> Handle(GetRoleAuthorityIDRequest request, CancellationToken cancellationToken)
        {
            //角色领域服务
            var roleService = new RoleService();
            //获取角色聚合
            var roleAgg = await roleService.GetAsync(request.RoleID);
            //权限ID集合
            var lstAuthorityID = roleAgg?.LstAuthorityID.Value!;

            return lstAuthorityID ?? new List<long>();
        }
    }
}
