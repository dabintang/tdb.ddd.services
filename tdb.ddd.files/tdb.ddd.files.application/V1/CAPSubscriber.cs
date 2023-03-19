using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.files.domain.contracts.Enum;
using tdb.ddd.files.domain.Files;
using tdb.ddd.files.infrastructure.Config;

namespace tdb.ddd.files.application.V1
{
    /// <summary>
    /// cap订阅者
    /// </summary>
    public class CAPSubscriber : ICAPSubscriber
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

        #region CAP

        /// <summary>
        /// 修改文件状态
        /// </summary>
        /// <param name="msg">修改文件状态 消息</param>
        /// <returns></returns>
        [CapSubscribe(TdbCst.CAPTopic.UpdateFilesStatus)]
        public async Task<UpdateFilesStatusRes> UpdateFilesStatusAsync(UpdateFilesStatusMsg msg)
        {
            var res = new UpdateFilesStatusRes();

            if (msg.LstFileStatus is null)
            {
                return res;
            }

            //循环更新文件状态
            foreach (var item in msg.LstFileStatus)
            {
                var updateResult = await this.UpdateFileStatusAsync(item, msg.OperatorID, msg.OperationTime);
                res.LstResult.Add(updateResult);
            }

            return res;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 修改文件状态
        /// </summary>
        /// <param name="req">条件</param>
        /// <param name="operatorID">操作人ID</param>
        /// <param name="operationTime">操作时间</param>
        /// <returns></returns>
        private async Task<UpdateFilesStatusRes.UpdateResult> UpdateFileStatusAsync(UpdateFilesStatusMsg.FileStatus req, long operatorID, DateTime operationTime)
        {
            var result = new UpdateFilesStatusRes.UpdateResult
            {
                ID = req.ID
            };

            //获取文件聚合
            var fileAgg = await this.FileService.GetFileAggAsync(req.ID);
            if (fileAgg == null)
            {
                result.IsSuccess = false;
                result.Msg = FilesConfig.Msg.FileNotExist.Msg;
                return result;
            }

            //如果文件状态未改变，直接返回成功
            if (fileAgg.FileStatusCode == req.FileStatusCode)
            {
                result.IsSuccess = true;
                result.Msg = TdbComResMsg.Success.Msg;
                return result;
            }

            //改为正式文件
            fileAgg.FileStatusCode = EnmFileStatus.Formal;
            fileAgg.UpdateInfo.UpdaterID = operatorID;
            fileAgg.UpdateInfo.UpdateTime = operationTime;

            //保存
            await this.FileService.SaveChangedAsync(fileAgg);

            result.IsSuccess = true;
            result.Msg = TdbComResMsg.Success.Msg;
            return result;
        }

        #endregion
    }

    /// <summary>
    /// cap订阅者
    /// </summary>
    public interface ICAPSubscriber : ICapSubscribe, ITdbIOCTransient
    {
        /// <summary>
        /// 修改文件状态
        /// </summary>
        /// <param name="msg">修改文件状态 消息</param>
        /// <returns></returns>
        Task<UpdateFilesStatusRes> UpdateFilesStatusAsync(UpdateFilesStatusMsg msg);
    }
}
