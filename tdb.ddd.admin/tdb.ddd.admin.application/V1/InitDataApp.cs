using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.application.BusMediatR;
using tdb.ddd.admin.application.contracts.Remote;
using tdb.ddd.admin.application.contracts.V1.Interface;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.admin.application.V1
{
    /// <summary>
    /// 初始化数据应用
    /// </summary>
    public class InitDataApp : IInitDataApp
    {
        #region 实现接口

        /// <summary>
        /// 初始化账户服务数据
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> InitAccountDataAsync()
        {
            //调用账户服务初始化数据接口 
            var accountApp = TdbIOC.GetService<IAccountApp>();
            var res = await accountApp!.InitDataAsync();

            //推送通知
            var msg = new InitDataNotification()
            {
                ServiceID = TdbCst.ServerID.Account,
                Result = res.Data ?? res.Msg,
                OperatorID = 0,
                OperationTime = DateTime.Now
            };
            TdbMediatR.Publish(msg);

            return res;
        }

        #endregion
    }
}
