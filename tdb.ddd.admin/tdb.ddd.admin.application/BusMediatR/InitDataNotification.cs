using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.domain.contracts.Enum;
using tdb.ddd.admin.domain.OperationRecord.Aggregate;
using tdb.ddd.admin.infrastructure;
using tdb.ddd.contracts;

namespace tdb.ddd.admin.application.BusMediatR
{
    /// <summary>
    /// 初始化数据 通知
    /// </summary>
    public class InitDataNotification : INotification
    {
        /// <summary>
        /// 服务ID
        /// </summary>
        public int ServiceID { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; } = "";

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long OperatorID { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }
    }

    /// <summary>
    /// 初始化数据记录日志
    /// </summary>
    public class InitDataNotificationLogHandler : INotificationHandler<InitDataNotification>
    {
        /// <summary>
        /// 初始化数据记录日志
        /// </summary>
        /// <param name="msg">初始化数据通知消息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(InitDataNotification msg, CancellationToken cancellationToken)
        {
            //操作记录聚合
            var agg = new OperationRecordAgg()
            {
                ID = AdminUniqueIDHelper.CreateID(),
                OperatorID = msg.OperatorID,
                OperationTime = msg.OperationTime
            };

            var sb = new StringBuilder("初始化");
            switch (msg.ServiceID)
            {
                case TdbCst.ServerID.Account:
                    agg.OperationTypeCode = EnmOperationType.RestoreAccountConfig;
                    sb.Append("账户服务");
                    break;
                default:
                    throw new TdbException($"初始化数据通知接收到未支持服务类型：{msg.ServiceID}");
            }
            sb.Append("数据：");
            sb.Append(msg.Result);

            agg.Content = sb.ToString();

            //保存
            await agg.SaveAsync();
        }
    }
}
