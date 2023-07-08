using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.domain.contracts.Photo;
using tdb.ddd.relationships.domain.Photo.Aggregate;

namespace tdb.ddd.relationships.domain.Photo
{
    /// <summary>
    /// 照片存储接口
    /// </summary>
    public interface IPhotoRepos : ITdbIOCScoped
    {
        /// <summary>
        /// 根据ID获取照片聚合
        /// </summary>
        /// <param name="photoID">照片ID</param>
        /// <returns></returns>
        Task<PhotoAgg?> GetByIDAsync(long photoID);

        /// <summary>
        /// 查询照片聚合
        /// </summary>
        /// <param name="param">条件</param>
        /// <returns></returns>
        Task<TdbPageRes<PhotoAgg>> QueryAsync(QueryPhotoParam param);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">照片聚合</param>
        Task SaveChangedAsync(PhotoAgg agg);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="agg">照片聚合</param>
        Task DeleteAsync(PhotoAgg agg);
    }
}
