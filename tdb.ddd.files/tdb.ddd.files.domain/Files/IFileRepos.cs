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
    /// 文件仓储接口
    /// </summary>
    public interface IFileRepos : ITdbIOCScoped, ITdbIOCIntercept
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">文件聚合</param>
        Task SaveAsync(FileAgg agg);

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

        //TODO：此类查询是否应该移到报表领域中处理？
        /// <summary>
        /// 查询文件集合
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        Task<TdbPageRes<FileAgg>> QueryFileAggsAsync(QueryFilesReq req);
    }
}
