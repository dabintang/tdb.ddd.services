using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.User;

namespace tdb.account.application.BusMediatR
{
    /// <summary>
    /// 用户登录后写登录记录
    /// </summary>
    public class UserLoginLogNotificationHandler : INotificationHandler<UserLoginNotification>
    {
        /// <summary>
        /// 用户登录后写登录记录
        /// </summary>
        /// <param name="notification">用户登录通知消息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(UserLoginNotification notification, CancellationToken cancellationToken)
        {
            var userService = new UserService();
            await userService.AddLoginRecordAsync(notification.User.ID, notification.ClientIP, notification.LoginTime);
        }
    }
}
