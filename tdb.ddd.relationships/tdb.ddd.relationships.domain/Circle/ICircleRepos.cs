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
        /// 根据人际圈ID获取人际圈聚合
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <returns></returns>
        Task<CircleAgg?> GetByIDAsync(long circleID);

        /// <summary>
        /// 获取成员数
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <returns>成员数</returns>
        Task<int> CountMembersAsync(long circleID);

        /// <summary>
        /// 获取成员信息
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <param name="personnelID">人员ID</param>
        /// <returns></returns>
        Task<MemberEntity?> GetMemberAsync(long circleID, long personnelID);

        /// <summary>
        /// 添加或修改成员信息
        /// </summary>
        /// <param name="entity">成员信息</param>
        Task SaveMemberAsync(MemberEntity entity);

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="entity">成员信息</param>
        Task DeleteMemberAsync(MemberEntity entity);

        /// <summary>
        /// 删除所有成员
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        Task DeleteAllMembersAsync(long circleID);

        /// <summary>
        /// 保存人际圈信息
        /// </summary>
        /// <param name="agg">人际圈聚合</param>
        Task SaveCircleAsync(CircleAgg agg);

        /// <summary>
        /// 删除人际圈信息
        /// </summary>
        /// <param name="agg">人际圈聚合</param>
        Task DeleteCircleAsync(CircleAgg agg);

    }
}
