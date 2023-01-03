using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.files.domain.contracts.DTO;
using tdb.ddd.files.domain.Files.Aggregate;
using tdb.ddd.infrastructure;

namespace tdb.ddd.files.domain.Files
{
    /// <summary>
    /// 文件领域服务
    /// </summary>
    public class FileService
    {
        #region 仓储

        private IFileRepos _fileRepos;
        /// <summary>
        /// 文件仓储
        /// </summary>
        private IFileRepos FileRepos
        {
            get
            {
                this._fileRepos ??= TdbIOC.GetService<IFileRepos>();
                return this._fileRepos;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 添加或修改文件
        /// </summary>
        /// <param name="agg">文件聚合</param>
        /// <returns></returns>
        public async Task SaveChangedAsync(FileAgg agg)
        {
            await this.FileRepos.SaveChangedAsync(agg);
        }

        /// <summary>
        /// 获取文件聚合
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns></returns>
        public async Task<FileAgg> GetFileAggAsync(long fileID)
        {
            return await this.FileRepos.GetFileAggAsync(fileID);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="agg">文件聚合</param>
        /// <returns></returns>
        public async Task DeleteFileAsync(FileAgg agg)
        {
            //从数据库总删除
            await this.FileRepos.DeleteFileAsync(agg.ID);
            //删除文件
            await agg.DeleteFile();
        }

        /// <summary>
        /// 查询文件集合
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<FileAgg>> QueryFileAggsAsync(QueryFilesReq req)
        {
            return await this.FileRepos.QueryFileAggsAsync(req);
        }

        #endregion
    }
}
