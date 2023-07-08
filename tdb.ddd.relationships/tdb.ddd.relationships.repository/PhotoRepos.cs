using Autofac.Extras.DynamicProxy;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.relationships.domain.contracts.Photo;
using tdb.ddd.relationships.domain.Photo;
using tdb.ddd.relationships.domain.Photo.Aggregate;
using tdb.ddd.relationships.repository.DBEntity;
using tdb.ddd.repository.sqlsugar;
using static tdb.ddd.contracts.TdbCst;

namespace tdb.ddd.relationships.repository
{
    /// <summary>
    /// 照片存储
    /// </summary>
    public class PhotoRepos : TdbRepository<PhotoInfo>, IPhotoRepos
    {
        #region 实现接口

        /// <summary>
        /// 根据ID获取照片聚合
        /// </summary>
        /// <param name="photoID">照片ID</param>
        /// <returns></returns>
        public async Task<PhotoAgg?> GetByIDAsync(long photoID)
        {
            var photoInfo = await this.GetByIdAsync(photoID);
            if (photoInfo is null)
            {
                return null;
            }

            //转为聚合
            var photoAgg = DBMapper.Map<PhotoInfo, PhotoAgg>(photoInfo);
            return photoAgg;
        }

        /// <summary>
        /// 查询照片聚合
        /// </summary>
        /// <param name="param">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<PhotoAgg>> QueryAsync(QueryPhotoParam param)
        {
            var query = this.AsQueryable();

            //[必须]人员ID
            query = query.Where(m => m.PersonnelID == param.PersonnelID);

            //排序
            if (param.LstSortItem is not null)
            {
                foreach (var sort in param.LstSortItem)
                {
                    switch (sort.FieldCode)
                    {
                        case QueryPhotoParam.EnmSortField.ID:
                            query = query.OrderBy(m => m.ID, sort.SortCode == EnmTdbSort.Asc ? OrderByType.Asc : OrderByType.Desc);
                            break;
                    }
                }
            }

            //查询
            var total = new RefAsync<int>();
            var lstInfo = await query.ToOffsetPageAsync(param.PageNO, param.PageSize, total);
            var lstAgg = DBMapper.Map<List<PhotoInfo>, List<PhotoAgg>>(lstInfo);

            return new TdbPageRes<PhotoAgg>(TdbComResMsg.Success, lstAgg, total);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">照片聚合</param>
        public async Task SaveChangedAsync(PhotoAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<PhotoAgg, PhotoInfo>(agg);

            //保存
            await this.InsertOrUpdateAsync(info);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="agg">照片聚合</param>
        public async Task DeleteAsync(PhotoAgg agg)
        {
            await this.DeleteByIdAsync(agg.ID);
        }

        #endregion
    }
}
