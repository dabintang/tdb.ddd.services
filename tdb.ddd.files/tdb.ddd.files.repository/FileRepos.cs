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
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.files.repository
{
    /// <summary>
    /// 临时文件仓储
    /// </summary>
    public class FileRepos : TdbRepository<DBEntity.FileInfo>, IFileRepos
    {
        /// <summary>
        /// 聚合备份
        /// （key：文件ID；value：聚合）
        /// </summary>
        private readonly Dictionary<long, FileAgg> dicBackup = new();

        #region 实现接口

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">文件聚合</param>
        public async Task SaveChangedAsync(FileAgg agg)
        {
            //获取备份
            var aggBackup = this.GetBackup(agg.ID);

            //文件信息是否有改动
            if (agg.Equals(aggBackup) == false)
            {
                //转换为数据库实体
                var info = DBMapper.Map<FileAgg, DBEntity.FileInfo>(agg);

                //如果有备份，则说明是更新操作
                if (aggBackup is not null)
                {
                    await this.AsUpdateable(info).ExecuteCommandAsync();
                }
                else
                {
                    await this.InsertOrUpdateAsync(info);
                }
                //清缓存
                TdbCache.Ins.HDel(Cst_CacheKeyFileInfo, agg.ID.ToStr());

                //备份
                this.Backup(agg);
            }
        }

        /// <summary>
        /// 获取文件聚合
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns></returns>
        public async Task<FileAgg?> GetFileAggAsync(long fileID)
        {
            //获取文件信息
            var fileInfo = await TdbCache.Ins.HCacheShellAsync(Cst_CacheKeyFileInfo, TimeSpan.FromDays(1), fileID.ToStr(), async () => await this.GetFirstAsync(m => m.ID == fileID));
            //var fileInfo = await this.GetFirstAsync(m => m.ID == fileID);
            if (fileInfo == null)
            {
                return null;
            }

            //转换为聚合结构
            var userAgg = DBMapper.Map<DBEntity.FileInfo, FileAgg>(fileInfo);
            //备份
            this.Backup(userAgg);

            return userAgg;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns></returns>
        public async Task DeleteFileAsync(long fileID)
        {
            //移除备份
            this.RemoveBackup(fileID);

            //清缓存
            TdbCache.Ins.HDel(Cst_CacheKeyFileInfo, fileID.ToStr());

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
            if (req.LstSortItem != null && req.LstSortItem.Count > 0)
            {
                //排序字符串
                var orderFields = string.Join(',', 
                                            req.LstSortItem.Where(m => string.IsNullOrWhiteSpace(m.FieldName) == false)
                                                           .Select(m => m.SortCode == TdbSortItem.EnmSort.Asc ? $"{m.FieldName}" : $"{m.FieldName} DESC"));
                query.OrderByIF(string.IsNullOrWhiteSpace(orderFields) == false, orderFields);
            }
            else
            {
                //默认按创建时间降序排序
                query.OrderByDescending(m => m.CreateTime);
            }

            //查询
            var total = new RefAsync<int>();
            var lstInfo = await query.ToOffsetPageAsync(req.PageNO, req.PageSize, total);
            var lstAgg = DBMapper.Map<List<DBEntity.FileInfo>, List<FileAgg>>(lstInfo);

            return new TdbPageRes<FileAgg>(TdbComResMsg.Success, lstAgg, total);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 备份
        /// </summary>
        /// <param name="agg">文件聚合</param>
        private void Backup(FileAgg agg)
        {
            var backup = agg.DeepClone();
            if (backup is not null)
            {
                this.dicBackup[agg.ID] = backup;
            }
        }

        /// <summary>
        /// 获取备份
        /// </summary>
        /// <param name="id">文件ID</param>
        private FileAgg? GetBackup(long id)
        {
            this.dicBackup.TryGetValue(id, out FileAgg? aggBackup);
            return aggBackup;
        }

        /// <summary>
        /// 移除备份
        /// </summary>
        /// <param name="id">文件ID</param>
        private void RemoveBackup(long id)
        {
            this.dicBackup.Remove(id);
        }

        #endregion

        #region 缓存key

        /// <summary>
        /// 【文件信息】缓存key
        /// </summary>
        private const string Cst_CacheKeyFileInfo = "ReposFileInfo";

        #endregion
    }
}
