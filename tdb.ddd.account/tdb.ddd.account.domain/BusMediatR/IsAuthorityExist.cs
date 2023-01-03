using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Authority;

namespace tdb.ddd.account.domain.BusMediatR
{
    /// <summary>
    /// 判断权限是否存在 请求参数
    /// </summary>
    public class IsAuthorityExistRequest : IRequest<bool>
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public long AuthorityID { get; set; }
    }

    /// <summary>
    /// 判断权限是否存在
    /// </summary>
    public class IsAuthorityExistRequestHandler : IRequestHandler<IsAuthorityExistRequest, bool>
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(IsAuthorityExistRequest request, CancellationToken cancellationToken)
        {
            var authorityService = new AuthorityService();
            return await authorityService.IsExist(request.AuthorityID);
        }
    }
}
