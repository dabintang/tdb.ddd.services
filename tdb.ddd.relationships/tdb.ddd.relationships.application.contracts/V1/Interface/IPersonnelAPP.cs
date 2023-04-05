using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO;

namespace tdb.ddd.relationships.application.contracts.V1.Interface
{
    /// <summary>
    /// 人员应用接口
    /// </summary>
    public interface IPersonnelAPP : ITdbIOCScoped
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新用户ID</returns>
        Task<TdbRes<AddPersonnelRes>> AddPersonnelAsync(TdbOperateReq<AddPersonnelReq> req);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        Task<TdbRes<bool>> UpdatePersonnelAsync(TdbOperateReq<UpdatePersonnelReq> req);
    }
}
