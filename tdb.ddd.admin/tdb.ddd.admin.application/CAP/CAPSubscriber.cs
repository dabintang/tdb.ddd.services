using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.application.contracts.CAP;
using tdb.ddd.admin.domain.OperationRecord.Aggregate;
using tdb.ddd.admin.infrastructure;
using tdb.ddd.contracts;

namespace tdb.ddd.admin.application.CAP
{
    /// <summary>
    /// cap订阅者
    /// </summary>
    public class CAPSubscriber : ICapSubscribe
    {
        #region CAP

        /// <summary>
        /// 用户登录通知
        /// </summary>
        /// <param name="msg">用户登录通知 消息</param>
        /// <returns></returns>
        [CapSubscribe(AdminCst.CAPTopic.UserLogin)]
        public async Task UserLoginAsync(UserLoginMsg msg)
        {
            //操作记录聚合
            var agg = new OperationRecordAgg()
            {
                ID = AdminUniqueIDHelper.CreateID(),
                OperatorID = msg.OperatorID,
                OperationTime = msg.OperationTime,
                OperationTypeCode = domain.contracts.Enum.EnmOperationType.UserLogin,
                Content = $"用户登录。姓名：{msg.OperatorName}，时间：{msg.OperationTime:yyyy-MM-dd HH:mm:ss}，IP：{msg.ClientIP}"
            };

            //保存
            await agg.SaveAsync();
        }

        #endregion
    }
}
