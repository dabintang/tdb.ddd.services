using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.domain.contracts.Personnel;
using tdb.ddd.relationships.domain.Personnel.Aggregate;

namespace tdb.ddd.relationships.domain.Personnel
{
    /// <summary>
    /// 人员存储接口
    /// </summary>
    public interface IPersonnelRepos : ITdbIOCScoped, ITdbIOCIntercept
    {
        /// <summary>
        /// 根据人员ID获取人员聚合
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <returns></returns>
        Task<PersonnelAgg?> GetByIDAsync(long personnelID);

        /// <summary>
        /// 根据用户ID获取人员聚合
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<PersonnelAgg?> GetByUserIDAsync(long userID);

        /// <summary>
        /// 获取人员的照片ID
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <returns></returns>
        Task<List<long>> GetPhotoIDsAsync(long personnelID);

        /// <summary>
        /// 获取人员的人际圈ID
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <returns></returns>
        Task<List<long>> GetCircleIDsAsync(long personnelID);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">人员聚合</param>
        Task SaveChangedAsync(PersonnelAgg agg);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="agg">人员聚合</param>
        Task DeleteAsync(PersonnelAgg agg);
    }
}
