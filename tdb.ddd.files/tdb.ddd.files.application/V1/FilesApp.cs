using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.application.contracts;
using tdb.ddd.domain;
using tdb.ddd.files.application.contracts.V1.Interface;
using tdb.ddd.files.application.contracts.V1.DTO;
using tdb.ddd.files.infrastructure.Config;
using tdb.ddd.infrastructure;
using tdb.ddd.files.domain.Files;
using tdb.ddd.files.domain.Files.Aggregate;
using tdb.ddd.files.infrastructure;
using tdb.ddd.contracts;
using ImageMagick;
using tdb.ddd.files.domain.contracts.Enum;
using tdb.ddd.files.domain.contracts.DTO;
using static tdb.ddd.files.domain.contracts.Const.Cst;
using tdb.ddd.files.domain.contracts.Const;

namespace tdb.ddd.files.application.V1
{
    /// <summary>
    /// 文件应用
    /// </summary>
    public class FilesApp : IFilesApp
    {
        #region 领域服务

        private FileService _fileService;
        /// <summary>
        /// 文件领域服务
        /// </summary>
        public FileService FileService
        {
            get
            {
                this._fileService ??= new FileService();
                return _fileService;
            }
        }

        #endregion

        #region 实现接口

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="reqDTO">文件信息</param>
        /// <returns></returns>
        public async Task<List<UploadFilesRes>> UploadFilesAsync(TdbOperateReq<List<UploadFilesReq>> reqDTO)
        {
            //结果
            var lstRes = new List<UploadFilesRes>();

            var files = reqDTO.Param;
            foreach (var file in files)
            {
                var res = new UploadFilesRes();
                lstRes.Add(res);
                res.Name = file.Name;

                try
                {
                    var agg = new FileAgg();
                    agg.ID = FilesUniqueIDHelper.CreateID();
                    agg.Name = file.Name;
                    agg.AccessLevelCode = file.AccessLevelCode;
                    agg.StorageTypeCode = file.StorageTypeCode;
                    agg.FileStatusCode = file.FileStatusCode;
                    agg.Remark = file.Remark;
                    agg.CreateInfo = new CreateInfoValueObject() { CreatorID = reqDTO.OperatorID, CreateTime = reqDTO.OperationTime };
                    agg.UpdateInfo = new UpdateInfoValueObject() { UpdaterID = reqDTO.OperatorID, UpdateTime = reqDTO.OperationTime };

                    //保存文件到物理路径
                    await agg.SaveFileAsync(file.Data);

                    //持久化
                    await this.FileService.SaveChangedAsync(agg);

                    res.ID = agg.ID;
                    res.IsOK = true;
                    res.Msg = "上传成功";
                }
                catch (Exception ex)
                {
                    TdbLogger.Ins.Error($"上传临时文件异常，file=[{file.SerializeJson()}]，ex=[{ex}]");

                    res.IsOK = false;
                    res.Msg = $"上传失败[{ex.Message}]";
                }
            }

            return lstRes;
        }

        /// <summary>
        /// 确认文件
        /// </summary>
        /// <param name="reqDTO">条件</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> ConfirmFileAsync(TdbOperateReq<ConfirmFileReq> reqDTO)
        {
            //获取文件聚合
            var fileAgg = await this.FileService.GetFileAggAsync(reqDTO.Param.ID);
            if (fileAgg == null)
            {
                return new TdbRes<bool>(FilesConfig.Msg.FileNotExist, false);
            }

            //是否有修改权限
            if (fileAgg.IsAuthorizedModify(reqDTO.OperatorID, reqDTO.OperatorRoleIDs) == false)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //如果已经是正式文件，直接返回成功
            if (fileAgg.FileStatusCode == EnmFileStatus.Formal)
            {
                return TdbRes.Success(true);
            }

            //改为正式文件
            fileAgg.FileStatusCode = domain.contracts.Enum.EnmFileStatus.Formal;
            fileAgg.UpdateInfo.UpdaterID = reqDTO.OperatorID;
            fileAgg.UpdateInfo.UpdateTime = reqDTO.OperationTime;

            //保存
            await this.FileService.SaveChangedAsync(fileAgg);

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="reqDTO">条件</param>
        /// <returns></returns>
        public async Task<TdbRes<DownloadFileRes>> DownloadFileAsync(TdbOperateReq<DownloadFileReq> reqDTO)
        {
            //获取文件聚合
            var fileAgg = await this.FileService.GetFileAggAsync(reqDTO.Param.ID);
            if (fileAgg == null)
            {
                return new TdbRes<DownloadFileRes>(FilesConfig.Msg.FileNotExist, null);
            }

            //是否有访问权限
            if (fileAgg.IsAuthorizedAccess(reqDTO.OperatorID, reqDTO.OperatorRoleIDs) == false)
            {
                return new TdbRes<DownloadFileRes>(TdbComResMsg.InsufficientPermissions, null);
            }

            var res = new DownloadFileRes();
            res.ID = fileAgg.ID;
            res.Name = fileAgg.Name;
            res.ContentType = fileAgg.GetContentType();
            res.Data = await fileAgg.ReadFileAsync();

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="reqDTO">条件</param>
        /// <returns></returns>
        public async Task<TdbRes<DownloadFileRes>> DownloadImageAsync(TdbOperateReq<DownloadImageReq> reqDTO)
        {
            //未调整大小，返回原图
            if (reqDTO.Param.Width <= 0 && reqDTO.Param.Height <= 0)
            {
                return await this.DownloadFileAsync(new TdbOperateReq<DownloadFileReq>()
                {
                    OperatorID = reqDTO.OperatorID,
                    OperatorName = reqDTO.OperatorName,
                    OperatorRoleIDs = reqDTO.OperatorRoleIDs,
                    OperatorAuthorityIDs = reqDTO.OperatorAuthorityIDs,
                    OperationTime = reqDTO.OperationTime,
                    Param = new DownloadFileReq() { ID = reqDTO.Param.ID }
                });
            }

            //获取文件聚合
            var fileAgg = await this.FileService.GetFileAggAsync(reqDTO.Param.ID);
            if (fileAgg == null)
            {
                return new TdbRes<DownloadFileRes>(FilesConfig.Msg.FileNotExist, null);
            }

            //是否有访问权限
            if (fileAgg.IsAuthorizedAccess(reqDTO.OperatorID, reqDTO.OperatorRoleIDs) == false)
            {
                return new TdbRes<DownloadFileRes>(TdbComResMsg.InsufficientPermissions, null);
            }

            //是否图片
            if (fileAgg.IsImage() == false)
            {
                return new TdbRes<DownloadFileRes>(FilesConfig.Msg.NotImage, null);
            }

            //生成缩略图
            var data = await fileAgg.ReadFileAsync();
            using (var img = new MagickImage(data))
            {
                //宽度
                int width = reqDTO.Param.Width > 0 ? reqDTO.Param.Width : (int)(((double)img.Width / img.Height) * reqDTO.Param.Height);
                //高度
                int height = reqDTO.Param.Height > 0 ? reqDTO.Param.Height : (int)(((double)img.Height / img.Width) * reqDTO.Param.Width);

                //生成缩略图
                img.Thumbnail(width, height);
                data = img.ToByteArray();
            }

            var res = new DownloadFileRes();
            res.ID = fileAgg.ID;
            res.Name = fileAgg.Name;
            res.ContentType = fileAgg.GetContentType();
            res.Data = data;

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        /// <param name="reqDTO">条件</param>
        /// <returns></returns>
        public async Task<TdbRes<DeleteTempFilesRes>> DeleteTempFilesAsync(TdbOperateReq<DeleteTempFilesReq> reqDTO)
        {
            //超级管理员才有批量删除临时文件权限
            if ((reqDTO.OperatorRoleIDs?.Contains(Cst.RoleID.SuperAdmin) ?? false) == false)
            {
                return new TdbRes<DeleteTempFilesRes>(TdbComResMsg.InsufficientPermissions, null);
            }

            //查询需要删除的临时文件
            var reqQuery = new QueryFilesReq()
            {
                PageNO = 1,
                PageSize = 2,
                FileStatusCode = (byte)EnmFileStatus.Temp,
                StartCreateTime = reqDTO.Param.StartCreateTime,
                EndCreateTime = reqDTO.Param.EndCreateTime
            };
            var list = await this.FileService.QueryFileAggsAsync(reqQuery);


            return null;
        }

        #endregion
    }
}
