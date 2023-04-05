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
    /// 照片领域服务
    /// </summary>
    public class PhotoService
    {
        #region 仓储

        private IPhotoRepos? _photoRepos;
        /// <summary>
        /// 照片仓储
        /// </summary>
        private IPhotoRepos PhotoRepos
        {
            get
            {
                this._photoRepos ??= TdbIOC.GetService<IPhotoRepos>();
                if (this._photoRepos is null)
                {
                    throw new TdbException("照片仓储接口未实现");
                }
                return this._photoRepos;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 根据ID获取照片聚合
        /// </summary>
        /// <param name="photoID">照片ID</param>
        /// <returns></returns>
        public async Task<PhotoAgg?> GetByIDAsync(long photoID)
        {
            //获取照片聚合
            var agg = await this.PhotoRepos.GetByIDAsync(photoID);
            return agg;
        }

        /// <summary>
        /// 查询照片聚合
        /// </summary>
        /// <param name="param">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<PhotoAgg>> QueryAsync(QueryPhotoParam param)
        {
            return await this.PhotoRepos.QueryAsync(param);
        }

        #endregion
    }
}
