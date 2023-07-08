using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.relationships.domain.Circle;
using tdb.ddd.relationships.domain.Circle.Aggregate;
using tdb.ddd.relationships.infrastructure;
using tdb.ddd.relationships.repository.DBEntity;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.relationships.repository
{
    /// <summary>
    /// 人际圈仓储
    /// </summary>
    [Intercept(typeof(TdbCacheInterceptor))]
    public class CircleRepos : TdbRepository<CircleInfo>, ICircleRepos
    {
        #region 实现接口

        /// <summary>
        /// 根据人际圈ID获取人际圈聚合
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(RelationshipsCst.CacheKey.HashCircleByID)]
        [TdbCacheKey(0)]
        public async Task<CircleAgg?> GetByIDAsync(long circleID)
        {
            var circleInfo = await this.GetByIdAsync(circleID);
            if (circleInfo is null)
            {
                return null;
            }

            //转为聚合
            var circleAgg = DBMapper.Map<CircleInfo, CircleAgg>(circleInfo);
            return circleAgg;
        }

        /// <summary>
        /// 获取成员数
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <returns>成员数</returns>
        public async Task<int> CountMembersAsync(long circleID)
        {
            return await this.Change<CircleMemberInfo>().AsQueryable().CountAsync(m => m.CircleID == circleID);
        }

        /// <summary>
        /// 获取成员信息
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <param name="personnelID">人员ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(RelationshipsCst.CacheKey.HashCircleMemberByCircleID_PersonnelID, ParamIndex = 0)]
        [TdbCacheKey(1)]
        public async Task<MemberEntity?> GetMemberAsync(long circleID, long personnelID)
        {
            var memberInfo = await this.Change<CircleMemberInfo>().AsQueryable().FirstAsync(m => m.CircleID == circleID && m.PersonnelID == personnelID);
            if (memberInfo is null)
            {
                return null;
            }

            //转为实体
            var memberEntity = DBMapper.Map<CircleMemberInfo, MemberEntity>(memberInfo);
            return memberEntity;
        }

        /// <summary>
        /// 添加或修改成员信息
        /// </summary>
        /// <param name="entity">成员信息</param>
        [TdbRemoveCacheHash(RelationshipsCst.CacheKey.HashCircleMemberByCircleID_PersonnelID, ParamIndex = 0, FromPropertyName = "CircleID")]
        [TdbCacheKey(0, FromPropertyName = "PersonnelID")]
        public async Task SaveMemberAsync(MemberEntity entity)
        {
            //转换为数据库实体
            var info = DBMapper.Map<MemberEntity, CircleMemberInfo>(entity);

            //保存
            await this.Change<CircleMemberInfo>().InsertOrUpdateAsync(info);
        }

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="entity">成员信息</param>
        [TdbRemoveCacheHash(RelationshipsCst.CacheKey.HashCircleMemberByCircleID_PersonnelID, ParamIndex = 0, FromPropertyName = "CircleID")]
        [TdbCacheKey(0, FromPropertyName = "PersonnelID")]
        public async Task DeleteMemberAsync(MemberEntity entity)
        {
            //删除
            await this.Change<CircleMemberInfo>().DeleteByIdAsync(entity.ID);
        }

        /// <summary>
        /// 删除所有成员
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        [TdbRemoveCacheString(RelationshipsCst.CacheKey.HashCircleMemberByCircleID_PersonnelID)]
        [TdbCacheKey(0)]
        public async Task DeleteAllMembersAsync(long circleID)
        {
            await this.Change<CircleMemberInfo>().DeleteAsync(m => m.CircleID == circleID);
        }

        /// <summary>
        /// 保存人际圈信息
        /// </summary>
        /// <param name="agg">人际圈聚合</param>
        [TdbRemoveCacheHash(RelationshipsCst.CacheKey.HashCircleByID)]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task SaveCircleAsync(CircleAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<CircleAgg, CircleInfo>(agg);

            //保存
            await this.InsertOrUpdateAsync(info);
        }

        /// <summary>
        /// 删除人际圈信息
        /// </summary>
        /// <param name="agg">人际圈聚合</param>
        [TdbRemoveCacheHash(RelationshipsCst.CacheKey.HashCircleByID)]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task DeleteCircleAsync(CircleAgg agg)
        {
            //删除
            await this.DeleteByIdAsync(agg.ID);
        }

        #endregion
    }
}
