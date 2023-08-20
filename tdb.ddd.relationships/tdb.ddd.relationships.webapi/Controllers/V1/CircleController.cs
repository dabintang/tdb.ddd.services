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
        public async Task<TdbRes<bool>> DeleteCircle([FromBody] DeleteCircleReq req)
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
        public async Task<TdbRes<bool>> AddMember([FromBody] AddMemberReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //添加成员
            var res = await this.circleAPP.AddMemberAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 批量添加成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<BatchAddMemberRes>> BatchAddMember([FromBody] BatchAddMemberReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //批量添加成员
            var res = await this.circleAPP.BatchAddMemberAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 移出成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> RemoveMember([FromBody] RemoveMemberReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //移出成员
            var res = await this.circleAPP.RemoveMemberAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 批量移出成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<BatchRemoveMemberRes>> BatchRemoveMember([FromBody] BatchRemoveMemberReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //移出成员
            var res = await this.circleAPP.BatchRemoveMemberAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 设置成员角色
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> SetMemberRole([FromBody] SetMemberRoleReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //设置成员角色
            var res = await this.circleAPP.SetMemberRoleAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 生成加入人际圈的邀请码
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<CreateInvitationCodeRes>> CreateInvitationCode([FromBody] CreateInvitationCodeReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //生成加入人际圈的邀请码
            var res = await this.circleAPP.CreateInvitationCodeAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 通过邀请码加入人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<JoinByInvitationCodeRes>> JoinByInvitationCode([FromBody] JoinByInvitationCodeReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //通过邀请码加入人际圈
            var res = await this.circleAPP.JoinByInvitationCodeAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 退出人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> WithdrawCircle([FromBody] WithdrawCircleReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //退出人际圈
            var res = await this.circleAPP.WithdrawCircleAsync(reqOpe);
            return res;
        }

        #endregion
    }
}
