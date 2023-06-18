using Autofac.Extras.DynamicProxy;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.files.domain.contracts.DTO;
using tdb.ddd.files.domain.Files;
using tdb.ddd.files.domain.Files.Aggregate;
using tdb.ddd.files.infrastructure;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.files.repository
{
    /// <summary>
    /// 临时文件仓储
    /// </summary>
    [Intercept(typeof(TdbCacheInterceptor))]
    public class FileRepos : TdbRepository<DBEntity.FileInfo>, IFileRepos
    {
        #region 实现接口

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">文件聚合</param>
        [TdbRemoveCacheHash(FilesCst.CacheKey.HashFileByID)]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task SaveAsync(FileAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<FileAgg, DBEntity.FileInfo>(agg);

            //保存
            await this.InsertOrUpdateAsync(info);
        }

        /// <summary>
        /// 获取文件聚合
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns></returns>
        [TdbReadCacheHash(FilesCst.CacheKey.HashFileByID)]
        [TdbCacheKey(0)]
        public async Task<FileAgg?> GetFileAggAsync(long fileID)
        {
            //获取文件信息
            var fileInfo = await this.GetFirstAsync(m => m.ID == fileID);
            if (fileInfo == null)
            {
                return null;
            }

            //转换为聚合结构
            var userAgg = DBMapper.Map<DBEntity.FileInfo, FileAgg>(fileInfo);
            return userAgg;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns></returns>
        [TdbRemoveCacheHash(FilesCst.CacheKey.HashFileByID)]
        [TdbCacheKey(0)]
        public async Task DeleteFileAsync(long fileID)
        {
            //从数据库删除
            await this.DeleteByIdAsync(fileID);
        }

        /// <summary>
        /// 查询文件集合
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<FileAgg>> QueryFileAggsAsync(QueryFilesReq req)
        {
            var query = this.AsQueryable();
            //[可选]文件状态（1：临时文件；2：正式文件）
            query.WhereIF(req.FileStatusCode.HasValue, m => m.FileStatusCode == req.FileStatusCode);
            //[可选]起始创建时间（大于等于）
            query.WhereIF(req.StartCreateTime.HasValue, m => m.CreateTime >= req.StartCreateTime);
            //[可选]截止创建时间（小于等于）
            query.WhereIF(req.EndCreateTime.HasValue, m => m.CreateTime <= req.EndCreateTime);
            //排序
            //if (req.LstSortItem != null && req.LstSortItem.Count > 0)
            //{
            //    //排序字符串
            //    var orderFields = string.Join(',', 
            //                                req.LstSortItem.Where(m => string.IsNullOrWhiteSpace(m.FieldName) == false)
            //                                               .Select(m => m.SortCode == EnmTdbSort.Asc ? $"{m.FieldName}" : $"{m.FieldName} DESC"));
            //    query.OrderByIF(string.IsNullOrWhiteSpace(orderFields) == false, orderFields);
            //}
            //else
            //{
                //默认按创建时间降序排序
                query.OrderByDescending(m => m.CreateTime);
            //}

            //查询
            var total = new RefAsync<int>();
            var lstInfo = await query.ToOffsetPageAsync(req.PageNO, req.PageSize, total);
            var lstAgg = DBMapper.Map<List<DBEntity.FileInfo>, List<FileAgg>>(lstInfo);

            return new TdbPageRes<FileAgg>(TdbComResMsg.Success, lstAgg, total);
        }

        #endregion
    }
}
