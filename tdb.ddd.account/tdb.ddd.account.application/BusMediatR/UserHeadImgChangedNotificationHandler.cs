using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.application.CAP;
using tdb.ddd.account.domain.BusMediatR;
using tdb.ddd.contracts;

namespace tdb.ddd.account.application.BusMediatR
{
    /// <summary>
    /// 用户头像改动通知处理
    /// </summary>
    public class UserHeadImgChangedNotificationHandler : INotificationHandler<UserHeadImgChangedNotification>
    {
        /// <summary>
        /// 用户头像改动通知处理
        /// </summary>
        /// <param name="msg">用户头像改动通知消息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(UserHeadImgChangedNotification msg, CancellationToken cancellationToken)
        {
            if (msg.OldHeadImgID == msg.NewHeadImgID)
            {
                //无改动，不做处理
                return;
            }

            var updateHeadImgStatusMsg = new UpdateFilesStatusMsg()
            {
                OperatorID = msg.OperatorID,
                OperationTime = msg.OperationTime
            };

            //如果更新前存在头像
            if (msg.OldHeadImgID is not null)
            {
                //把原头像状态设置为临时
                updateHeadImgStatusMsg.LstFileStatus.Add(new UpdateFilesStatusMsg.FileStatus() { ID = msg.OldHeadImgID.Value, FileStatusCode = EnmTdbFileStatus.Temp });
            }
            //如果有新头像
            if (msg.NewHeadImgID is not null)
            {
                updateHeadImgStatusMsg.LstFileStatus.Add(new UpdateFilesStatusMsg.FileStatus() { ID = msg.NewHeadImgID.Value, FileStatusCode = EnmTdbFileStatus.Formal });
            }

            if (updateHeadImgStatusMsg.LstFileStatus.Count > 0)
            {
                await CAPPublisher.UpdateFilesStatusAsync(updateHeadImgStatusMsg);
            }
        }
    }
}
