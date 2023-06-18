using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.application.CAP;
using tdb.ddd.account.application.contracts.CAP;
using tdb.ddd.account.domain.User;
using tdb.ddd.account.domain.User.Aggregate;

namespace tdb.ddd.account.application.BusMediatR
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

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="user">用户聚合信息</param>
        /// <param name="clientIP">客户端IP</param>
        /// <param name="loginTime">登录时间</param>
        public UserLoginNotification(UserAgg user, string clientIP, DateTime loginTime)
        {
            this.User = user;
            this.ClientIP = clientIP;
            this.LoginTime = loginTime;
        }
    }

    /// <summary>
    /// 用户登录后写登录记录
    /// </summary>
    public class UserLoginLogNotificationHandler : INotificationHandler<UserLoginNotification>
    {
        /// <summary>
        /// 用户登录后推送CAP通知
        /// </summary>
        /// <param name="msg">用户登录通知消息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(UserLoginNotification msg, CancellationToken cancellationToken)
        {
            var capMsg = new UserLoginMsg()
            {
                ClientIP = msg.ClientIP,
                OperatorID = msg.User.ID,
                OperatorName = msg.User.Name,
                OperationTime = msg.LoginTime
            };

            await CAPPublisher.UserLoginAsync(capMsg);
        }
    }
}
