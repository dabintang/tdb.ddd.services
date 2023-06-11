using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO.Report;
using tdb.ddd.relationships.application.contracts.V1.Interface;

namespace tdb.ddd.relationships.application.V1
{
    /// <summary>
    /// 报表应用
    /// </summary>
    public class ReportAPP : IReportAPP
    {
        #region 实现接口

        /// <summary>
        /// 查询人际圈列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<QueryCircleListRes>> QueryCircleListAsync(QueryCircleListReq req)
        {
            await Task.CompletedTask;
            return new TdbPageRes<QueryCircleListRes>(TdbComResMsg.Fail, new List<QueryCircleListRes>(), 0);
        }

        /// <summary>
        /// 查询人际圈内人员信息列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<QueryCirclePersonnelListRes>> QueryCirclePersonnelListAsync(QueryCirclePersonnelListReq req)
        {
            await Task.CompletedTask;
            return new TdbPageRes<QueryCirclePersonnelListRes>(TdbComResMsg.Fail, new List<QueryCirclePersonnelListRes>(), 0);
        }

        /// <summary>
        /// 查询成员信息列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<QueryPersonnelListRes>> QueryPersonnelListAsync(QueryPersonnelListReq req)
        {
            await Task.CompletedTask;
            return new TdbPageRes<QueryPersonnelListRes>(TdbComResMsg.Fail, new List<QueryPersonnelListRes>(), 0);
        }

        #endregion
    }
}
