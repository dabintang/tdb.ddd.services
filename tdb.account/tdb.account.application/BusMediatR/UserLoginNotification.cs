using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.User.Aggregate;

namespace tdb.account.application.BusMediatR
{
    /// <summary>
    /// 用户登录 通知
    /// </summary>
    public class UserLoginNotification : INotification
    {
        /// <summary>
        /// 用户聚合信息
        /// </summary>
        public UserAgg User { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
    }
}
