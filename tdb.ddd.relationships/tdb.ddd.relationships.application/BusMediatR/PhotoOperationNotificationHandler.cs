using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.CAP;
using tdb.ddd.relationships.domain.BusMediatR;

namespace tdb.ddd.relationships.application.BusMediatR
{
    /// <summary>
    /// 照片操作通知处理
    /// </summary>
    public class PhotoOperationNotificationHandler : INotificationHandler<PhotoOperationNotification>
    {
        /// <summary>
        /// 照片操作通知处理
        /// </summary>
        /// <param name="msg">照片操作通知消息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(PhotoOperationNotification msg, CancellationToken cancellationToken)
        {
            var capMsg = new UpdateFilesStatusMsg()
            {
                OperatorID = msg.OperatorID,
                OperationTime = msg.OperationTime
            };

            switch (msg.OperationTypeCode)
            {
                case PhotoOperationNotification.EnmOperationType.Save:
                    capMsg.LstFileStatus.Add(new UpdateFilesStatusMsg.FileStatus() { ID = msg.PhotoID, FileStatusCode = EnmTdbFileStatus.Formal });
                    break;
                case PhotoOperationNotification.EnmOperationType.Delete:
                    capMsg.LstFileStatus.Add(new UpdateFilesStatusMsg.FileStatus() { ID = msg.PhotoID, FileStatusCode = EnmTdbFileStatus.Temp });
                    break;
                default:
                    throw new TdbException($"未知照片操作类型：{msg.OperationTypeCode}");
            }

            if (capMsg.LstFileStatus.Count > 0)
            {
                await CAPPublisher.UpdateFilesStatusAsync(capMsg);
            }
        }
    }
}
