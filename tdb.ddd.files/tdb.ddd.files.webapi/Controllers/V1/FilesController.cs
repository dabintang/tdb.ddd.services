using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using tdb.ddd.contracts;
using tdb.ddd.files.application.contracts.V1.DTO;
using tdb.ddd.files.application.contracts.V1.Interface;
using tdb.ddd.files.domain.contracts.Enum;
using tdb.ddd.webapi;

namespace tdb.ddd.files.webapi.Controllers.V1
{
    /// <summary>
    /// 文件服务
    /// </summary>
    [TdbApiVersion(1)]
    public class FilesController : BaseController
    {
        /// <summary>
        /// 文件应用
        /// </summary>
        private readonly IFilesApp filesApp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filesApp">文件应用</param>
        public FilesController(IFilesApp filesApp)
        {
            this.filesApp = filesApp;
        }

        #region 接口

        /// <summary>
        /// 上传为临时文件
        /// 附加参数：
        /// 【[可选]，默认1]AccessLevelCode：访问级别(1：仅创建者；2：授权；3：公开)】、
        /// 【[可选]，默认1]StorageTypeCode：存储类型（1：本地磁盘）】、
        /// 【[可选]Remark：备注】
        /// </summary>
        /// <param name="formData">表单信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<TdbRes<List<UploadFilesRes>>> UploadTempFiles([FromForm] IFormCollection formData)
        {
            if (formData.Files.Count == 0)
            {
                return new TdbRes<List<UploadFilesRes>>(TdbComResMsg.InvalidParam.FromNewMsg("请求中未包含任何文件"), null);
            }

            //访问级别(1：仅创建者；2：授权；3：公开)
            formData.TryGetValue("AccessLevelCode", out StringValues strAccessLevelCode);
            if (string.IsNullOrWhiteSpace(strAccessLevelCode))
            {
                strAccessLevelCode = "1";
            }
            else if (strAccessLevelCode != "1" && strAccessLevelCode != "3")
            {
                return new TdbRes<List<UploadFilesRes>>(TdbComResMsg.InvalidParam.FromNewMsg("访问级别当前只支持：1：仅创建者；3：公开"), null);
            }
            var accessLevelCode = (EnmAccessLevel)Convert.ToByte(strAccessLevelCode);

            //存储类型（1：本地磁盘）
            formData.TryGetValue("StorageTypeCode", out StringValues strStorageTypeCode);
            if (string.IsNullOrWhiteSpace(strStorageTypeCode))
            {
                strStorageTypeCode = "1";
            }
            else if (strStorageTypeCode != "1")
            {
                return new TdbRes<List<UploadFilesRes>>(TdbComResMsg.InvalidParam.FromNewMsg("存储类型只支持：1：本地磁盘"), null);
            }
            var storageTypeCode = (EnmStorageType)Convert.ToByte(strStorageTypeCode);

            ////文件状态（1：临时文件；2：正式文件）
            //formData.TryGetValue("FileStatusCode", out StringValues strFileStatusCode);
            //if (string.IsNullOrWhiteSpace(strFileStatusCode))
            //{
            //    strFileStatusCode = "1";
            //}
            //else if (strFileStatusCode != "1" && strFileStatusCode != "2")
            //{
            //    return new TdbRes<List<UploadFilesRes>>(TdbComResMsg.InvalidParam.FromNewMsg("文件状态只支持：1：临时文件；2：正式文件"), null);
            //}
            //var fileStatusCode = (EnmFileStatus)Convert.ToByte(strFileStatusCode);
            var fileStatusCode = EnmFileStatus.Temp;

            //备注
            formData.TryGetValue("Remark", out StringValues strRemark);
            var remark = Convert.ToString(strRemark) ?? "";

            //请求参数
            var lstReq = new List<UploadFilesReq>();
            foreach (var file in formData.Files)
            {
                using (var stream = file.OpenReadStream())
                {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);

                    var req = new UploadFilesReq()
                    {
                        Name = file.FileName,
                        AccessLevelCode = accessLevelCode,
                        StorageTypeCode = storageTypeCode,
                        FileStatusCode = fileStatusCode,
                        Remark = remark,
                        Data = data
                    };
                    lstReq.Add(req);
                }
            }
            var reqOpe = this.CreateTdbOperateReq(lstReq);

            //上传文件
            var result = await this.filesApp.UploadFilesAsync(reqOpe);

            return TdbRes.Success(result);
        }

        /// <summary>
        /// 确认文件（从临时文件转为正式文件）
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<TdbRes<bool>> ConfirmFile([FromBody] ConfirmFileReq req)
        {
            //请求参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //确认文件
            var result = await this.filesApp.ConfirmFileAsync(reqOpe);
            return result;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [ResponseCache(Duration = 3600 * 24 * 30, VaryByQueryKeys = new string[] { "ID" })]
        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery] DownloadFileReq req)
        {
            //请求参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //下载文件
            var result = await this.filesApp.DownloadFileAsync(reqOpe);
            if (result.Code != TdbComResMsg.Success.Code)
            {
                return new JsonResult(result);
            }

            //返回文件
            var fileResult = new FileContentResult(result.Data.Data, result.Data.ContentType);
            fileResult.FileDownloadName = result.Data.Name;
            return fileResult;
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [ResponseCache(Duration = 3600 * 24 * 30, VaryByQueryKeys = new string[] { "ID", "Width", "Height" })]
        [HttpGet]
        public async Task<IActionResult> DownloadImage([FromQuery] DownloadImageReq req)
        {
            //请求参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //下载图片
            var result = await this.filesApp.DownloadImageAsync(reqOpe);
            if (result.Code != TdbComResMsg.Success.Code)
            {
                return new JsonResult(result);
            }

            //返回文件
            var fileResult = new FileContentResult(result.Data.Data, result.Data.ContentType);
            fileResult.FileDownloadName = result.Data.Name;
            return fileResult;
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<TdbRes<DeleteTempFilesRes>> DeleteTempFiles([FromBody] DeleteTempFilesReq req)
        {
            //请求参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //删除临时文件
            var result = await this.filesApp.DeleteTempFilesAsync(reqOpe);
            return result;
        }

        #endregion
    }
}
