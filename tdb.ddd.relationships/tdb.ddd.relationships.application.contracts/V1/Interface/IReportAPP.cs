using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO.Report;

namespace tdb.ddd.relationships.application.contracts.V1.Interface
{
    /// <summary>
    /// 报表应用接口
    /// </summary>
    public interface IReportAPP : ITdbIOCScoped
    {
        /// <summary>
        /// 查询人际圈列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        Task<TdbPageRes<QueryCircleListRes>> QueryCircleListAsync(QueryCircleListReq req);

        /// <summary>
        /// 查询人际圈内成员信息列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        Task<TdbPageRes<QueryCirclePersonnelListRes>> QueryCirclePersonnelListAsync(QueryCirclePersonnelListReq req);

        /// <summary>
        /// 查询成员信息列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        Task<TdbPageRes<QueryPersonnelListRes>> QueryPersonnelListAsync(QueryPersonnelListReq req);
    }
}
