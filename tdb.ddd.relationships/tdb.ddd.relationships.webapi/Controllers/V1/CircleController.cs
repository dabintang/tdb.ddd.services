using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO.Circle;
using tdb.ddd.relationships.application.contracts.V1.Interface;
using tdb.ddd.webapi;

namespace tdb.ddd.relationships.webapi.Controllers.V1
{
    /// <summary>
    /// 人际圈
    /// </summary>
    [TdbApiVersion(1)]
    public class CircleController : BaseController
    {
        /// <summary>
        /// 人际圈应用
        /// </summary>
        private readonly ICircleAPP circleAPP;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="circleAPP">人际圈应用</param>
        public CircleController(ICircleAPP circleAPP)
        {
            this.circleAPP = circleAPP;
        }

        #region 接口

        /// <summary>
        /// 获取人际圈信息
        /// </summary>
        /// <param name="req">参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TdbRes<GetCircleRes>> GetCircle([FromQuery] GetCircleReq req)
        {
            var res = await this.circleAPP.GetCircleAsync(req);
            return res;
        }

        /// <summary>
        /// 创建人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<AddCircleRes>> AddCircle([FromBody] AddCircleReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //创建人际圈
            var res = await this.circleAPP.AddCircleAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 更新人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> UpdateCircle([FromBody] UpdateCircleReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //更新人际圈
            var res = await this.circleAPP.UpdateCircleAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 解散人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> DeleteCircle(DeleteCircleReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //解散人际圈
            var res = await this.circleAPP.DeleteCircleAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> AddMember(AddMemberReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //添加成员
            var res = await this.circleAPP.AddMemberAsync(reqOpe);
            return res;
        }

        #endregion
    }
}
