using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.relationships.domain.Personnel;
using tdb.ddd.relationships.domain.Personnel.Aggregate;
using tdb.ddd.relationships.infrastructure;
using tdb.ddd.relationships.repository.DBEntity;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.relationships.repository
{
    /// <summary>
    /// 人员存储
    /// </summary>
    [Intercept(typeof(TdbCacheInterceptor))]
    public class PersonnelRepos : TdbRepository<PersonnelInfo>, IPersonnelRepos
    {
        #region 实现接口

        /// <summary>
        /// 根据人员ID获取人员聚合
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(RelationshipsCst.CacheKey.HashPersonnelByID)]
        [TdbCacheKey(0)]
        public async Task<PersonnelAgg?> GetByIDAsync(long personnelID)
        {
            var personnelInfo = await this.GetByIdAsync(personnelID);
            if (personnelInfo is null)
            {
                return null;
            }

            //转为聚合
            var personnelAgg = DBMapper.Map<PersonnelInfo, PersonnelAgg>(personnelInfo);
            return personnelAgg;
        }

        /// <summary>
        /// 根据用户ID获取人员聚合
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(RelationshipsCst.CacheKey.HashPersonnelByUserID)]
        [TdbCacheKey(0)]
        public async Task<PersonnelAgg?> GetByUserIDAsync(long userID)
        {
            var personnelInfo = await this.GetFirstAsync(m => m.UserID == userID);
            if (personnelInfo is null)
            {
                return null;
            }

            //转为聚合
            var personnelAgg = DBMapper.Map<PersonnelInfo, PersonnelAgg>(personnelInfo);
            return personnelAgg;
        }

        /// <summary>
        /// 获取人员的照片ID
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <returns></returns>
        public async Task<List<long>> GetPhotoIDsAsync(long personnelID)
        {
            var list = await this.Change<PhotoInfo>().AsQueryable().Where(m => m.PersonnelID == personnelID).Select(m => m.ID).ToListAsync();
            return list ?? new List<long>();
        }

        /// <summary>
        /// 获取人员的人际圈ID
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <returns></returns>
        public async Task<List<long>> GetCircleIDsAsync(long personnelID)
        {
            var list = await this.Change<CircleMemberInfo>().AsQueryable().Where(m => m.PersonnelID == personnelID).Select(m => m.ID).ToListAsync();
            return list ?? new List<long>();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">人员聚合</param>
        [TdbRemoveCacheHash(RelationshipsCst.CacheKey.HashPersonnelByID)]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task SaveChangedAsync(PersonnelAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<PersonnelAgg, PersonnelInfo>(agg);

            //保存
            await this.InsertOrUpdateAsync(info);

            //移除用用户ID作为key的缓存
            TdbCache.Ins.HDel(RelationshipsCst.CacheKey.HashPersonnelByUserID, agg.UserID.ToStr());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="agg">人员聚合</param>
        /// <returns></returns>
        [TdbRemoveCacheHash(RelationshipsCst.CacheKey.HashPersonnelByID)]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task DeleteAsync(PersonnelAgg agg)
        {
            //删除
            await this.DeleteByIdAsync(agg.ID);

            //移除用用户ID作为key的缓存
            TdbCache.Ins.HDel(RelationshipsCst.CacheKey.HashPersonnelByUserID, agg.UserID.ToStr());
        }

        #endregion
    }
}
