using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.application.contracts.CAP;
using tdb.ddd.account.infrastructure;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.account.application.CAP
{
    /// <summary>
    /// cap发布者
    /// </summary>
    public class CAPPublisher
    {
        /// <summary>
        /// 更新文件状态
        /// </summary>
        /// <param name="msg">cap消息</param>
        /// <returns></returns>
        public static async Task UpdateFilesStatusAsync(UpdateFilesStatusMsg msg)
        {
            await TdbCAP.PublishAsync(TdbCst.CAPTopic.UpdateFilesStatus, msg);
        }

        /// <summary>
        /// 用户登录通知
        /// </summary>
        /// <param name="msg">cap消息</param>
        /// <returns></returns>
        public static async Task UserLoginAsync(UserLoginMsg msg)
        {
            await TdbCAP.PublishAsync(AccountCst.CAPTopic.UserLogin, msg);
        }
    }
}
