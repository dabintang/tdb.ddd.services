using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.files.domain.contracts.DTO;
using tdb.ddd.files.domain.Files.Aggregate;

namespace tdb.ddd.files.domain.Files
{
    /// <summary>
    /// 文件仓储接口
    /// </summary>
    public interface IFileRepos : ITdbIOCScoped
    {
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">文件聚合</param>
        Task SaveChangedAsync(FileAgg agg);

        /// <summary>
        /// 获取文件聚合
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns></returns>
        Task<FileAgg?> GetFileAggAsync(long fileID);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns></returns>
        Task DeleteFileAsync(long fileID);

        /// <summary>
        /// 查询文件集合
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        Task<TdbPageRes<FileAgg>> QueryFileAggsAsync(QueryFilesReq req);
    }
}
