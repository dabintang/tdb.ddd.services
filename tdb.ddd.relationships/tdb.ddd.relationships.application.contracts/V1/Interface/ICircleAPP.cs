using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO.Circle;

namespace tdb.ddd.relationships.application.contracts.V1.Interface
{
    /// <summary>
    /// 人际圈应用接口
    /// </summary>
    public interface ICircleAPP : ITdbIOCScoped
    {
        /// <summary>
        /// 获取人际圈信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<GetCircleRes>> GetCircleAsync(GetCircleReq req);

        /// <summary>
        /// 创建人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        Task<TdbRes<AddCircleRes>> AddCircleAsync(TdbOperateReq<AddCircleReq> req);

        /// <summary>
        /// 更新人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        Task<TdbRes<bool>> UpdateCircleAsync(TdbOperateReq<UpdateCircleReq> req);

        /// <summary>
        /// 解散人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        Task<TdbRes<bool>> DeleteCircleAsync(TdbOperateReq<DeleteCircleReq> req);

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> AddMemberAsync(TdbOperateReq<AddMemberReq> req);

        /// <summary>
        /// 批量添加成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<BatchAddMemberRes>> BatchAddMemberAsync(TdbOperateReq<BatchAddMemberReq> req);

        /// <summary>
        /// 移出成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> RemoveMemberAsync(TdbOperateReq<RemoveMemberReq> req);

        /// <summary>
        /// 设置成员角色
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> SetMemberRoleAsync(TdbOperateReq<SetMemberRoleReq> req);

        /// <summary>
        /// 设置成员身份
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> SetMemberIdentityAsync(TdbOperateReq<SetMemberIdentityReq> req);

        /// <summary>
        /// 生成加入人际圈的邀请码
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<CreateInvitationCodeRes>> CreateInvitationCodeAsync(TdbOperateReq<CreateInvitationCodeReq> req);

        /// <summary>
        /// 读取加入人际圈的邀请码信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<ReadInvitationCodeRes>> ReadInvitationCodeAsync(TdbOperateReq<ReadInvitationCodeReq> req);

        /// <summary>
        /// 通过邀请码加入人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> JoinByInvitationCodeAsync(TdbOperateReq<JoinByInvitationCodeReq> req);

        /// <summary>
        /// 退出人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        //Task<TdbRes<bool>> WithdrawCircleAsync(TdbOperateReq<WithdrawCircleReq> req);
    }
}
