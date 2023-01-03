using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.files.application.contracts.V1.DTO;

namespace tdb.ddd.files.application.contracts.V1.Interface
{
    /// <summary>
    /// 文件应用接口
    /// </summary>
    public interface IFilesApp : ITdbIOCScoped
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="files">文件信息</param>
        /// <returns></returns>
        Task<List<UploadFilesRes>> UploadFilesAsync(TdbOperateReq<List<UploadFilesReq>> files);

        /// <summary>
        /// 确认文件
        /// </summary>
        /// <param name="reqDTO">条件</param>
        /// <returns></returns>
        Task<TdbRes<bool>> ConfirmFileAsync(TdbOperateReq<ConfirmFileReq> reqDTO);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="reqDTO">条件</param>
        /// <returns></returns>
        Task<TdbRes<DownloadFileRes>> DownloadFileAsync(TdbOperateReq<DownloadFileReq> reqDTO);

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="reqDTO">条件</param>
        /// <returns></returns>
        Task<TdbRes<DownloadFileRes>> DownloadImageAsync(TdbOperateReq<DownloadImageReq> reqDTO);

        /// <summary>
        /// 删除临时文件
        /// </summary>
        /// <param name="reqDTO">条件</param>
        /// <returns></returns>
        Task<TdbRes<DeleteTempFilesRes>> DeleteTempFilesAsync(TdbOperateReq<DeleteTempFilesReq> reqDTO);
    }
}
