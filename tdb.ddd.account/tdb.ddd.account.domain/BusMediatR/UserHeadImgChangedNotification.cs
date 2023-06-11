using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.User;

namespace tdb.ddd.account.domain.BusMediatR
{
    /// <summary>
    /// 用户头像改动通知消息
    /// </summary>
    public class UserHeadImgChangedNotification : INotification
    {
        /// <summary>
        /// 原头像图片ID
        /// </summary>
        public long? OldHeadImgID { get; set; }

        /// <summary>
        /// 新头像图片ID
        /// </summary>
        public long? NewHeadImgID { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public long OperatorID { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }
    }
}
