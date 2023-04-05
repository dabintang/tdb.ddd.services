using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.relationships.domain.Circle.Aggregate;

namespace tdb.ddd.relationships.domain.Circle
{
    /// <summary>
    /// 人际圈仓储接口
    /// </summary>
    public interface ICircleRepos : ITdbIOCScoped
    {
        /// <summary>
        /// 获取人员ID集合
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <returns></returns>
        Task<List<long>> GetPersonnelIDsAsync(long circleID);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">人际圈聚合</param>
        Task SaveChangedAsync(CircleAgg agg);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="personnelID">人际圈ID</param>
        /// <returns></returns>
        Task DeleteAsync(long circleID);

    }
}
