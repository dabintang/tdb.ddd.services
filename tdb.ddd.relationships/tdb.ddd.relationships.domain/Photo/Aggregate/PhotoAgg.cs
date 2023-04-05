﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.ddd.relationships.domain.Photo.Aggregate
{
    /// <summary>
    /// 照片聚合（从人员聚合拆分出来）
    /// </summary>
    public class PhotoAgg : TdbAggregateRoot<long>
    {
        #region 仓储

        private IPhotoRepos? _repos;
        /// <summary>
        /// 照片仓储
        /// </summary>
        private IPhotoRepos Repos
        {
            get
            {
                this._repos ??= TdbIOC.GetService<IPhotoRepos>();
                if (this._repos is null)
                {
                    throw new TdbException("照片仓储接口未实现");
                }
                return this._repos;
            }
        }

        #endregion

        #region 值

        /// <summary>
        /// 人员ID
        /// </summary>
        public long PersonnelID { get; set; }

        /// <summary>
        /// 创建信息
        /// </summary>
        public CreateInfoValueObject CreateInfo { get; set; } = new CreateInfoValueObject();

        /// <summary>
        /// 更新信息
        /// </summary>
        public UpdateInfoValueObject UpdateInfo { get; set; } = new UpdateInfoValueObject();

        #endregion

        #region 行为

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangedAsync()
        {
            await this.Repos.SaveChangedAsync(this);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            await this.Repos.DeleteAsync(this.ID);
        }

        #endregion

        #region 私有方法

        #endregion
    }
}
