using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO.Report;
using tdb.ddd.relationships.application.contracts.V1.Interface;
using tdb.ddd.webapi;

namespace tdb.ddd.relationships.webapi.Controllers.V1
{
    /// <summary>
    /// 报表
    /// </summary>
    [TdbApiVersion(1)]
    public class ReportController : BaseController
    {
        /// <summary>
        /// 报表应用
        /// </summary>
        private readonly IReportAPP reportAPP;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reportAPP">报表应用</param>
        public ReportController(IReportAPP reportAPP)
        {
            this.reportAPP = reportAPP;
        }

        #region 接口

        /// <summary>
        /// 查询人际圈列表
        /// </summary>
        /// <param name="req">参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TdbPageRes<QueryCircleListRes>> QueryCircleList([FromQuery] QueryCircleListReq req)
        {
            var res = await this.reportAPP.QueryCircleListAsync(req);
            return res;
        }

        /// <summary>
        /// 查询我加入了的人际圈列表
        /// </summary>
        /// <param name="req">参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TdbPageRes<QueryMyCircleListRes>> QueryMyCircleList([FromQuery] QueryMyCircleListReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            var res = await this.reportAPP.QueryMyCircleListAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 查询人际圈内成员信息列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TdbPageRes<QueryCirclePersonnelListRes>> QueryCirclePersonnelList([FromQuery] QueryCirclePersonnelListReq req)
        {
            var res = await this.reportAPP.QueryCirclePersonnelListAsync(req);
            return res;
        }

        /// <summary>
        /// 查询我创建的人员信息列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TdbPageRes<QueryMyPersonnelListRes>> QueryMyPersonnelList([FromQuery] QueryMyPersonnelListReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            var res = await this.reportAPP.QueryMyPersonnelListAsync(reqOpe);
            return res;
        }

        #endregion
    }
}
