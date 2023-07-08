using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO.Personnel;

namespace tdb.ddd.relationships.application.contracts.V1.Interface
{
    /// <summary>
    /// 人员应用接口
    /// </summary>
    public interface IPersonnelAPP : ITdbIOCScoped
    {
        /// <summary>
        /// 获取人员信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<GetPersonnelRes>> GetPersonnelAsync(GetPersonnelReq req);

        /// <summary>
        /// 根据用户ID获取人员信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<GetPersonnelByUserIDRes>> GetPersonnelByUserIDAsync(GetPersonnelByUserIDReq req);

        /// <summary>
        /// 创建我的人员信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        Task<TdbRes<CreateMyPersonnelInfoRes>> CreateMyPersonnelInfoAsync(TdbOperateReq<CreateMyPersonnelInfoReq> req);

        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        Task<TdbRes<AddPersonnelRes>> AddPersonnelAsync(TdbOperateReq<AddPersonnelReq> req);

        /// <summary>
        /// 更新人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> UpdatePersonnelAsync(TdbOperateReq<UpdatePersonnelReq> req);

        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> DeletePersonnelAsync(TdbOperateReq<DeletePersonnelReq> req);

        /// <summary>
        /// 添加人员照片
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> AddPersonnelPhotoAsync(TdbOperateReq<AddPersonnelPhotoReq> req);
    }
}
