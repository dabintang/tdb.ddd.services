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
    /// 还原配置 通知
    /// </summary>
    public class RestoreConfigNotification : INotification
    {
        /// <summary>
        /// 服务ID
        /// </summary>
        public int ServiceID { get; set; }

        /// <summary>
        /// json文件路径
        /// </summary>
        public string JsonPath { get; set; } = "";

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
    /// 还原配置记录日志
    /// </summary>
    public class RestoreConfigNotificationLogHandler : INotificationHandler<RestoreConfigNotification>
    {
        /// <summary>
        /// 还原配置记录日志
        /// </summary>
        /// <param name="msg">还原配置通知消息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(RestoreConfigNotification msg, CancellationToken cancellationToken)
        {
            //操作记录聚合
            var agg = new OperationRecordAgg()
            {
                ID = AdminUniqueIDHelper.CreateID(),
                OperatorID = msg.OperatorID,
                OperationTime = msg.OperationTime
            };

            var sb = new StringBuilder("还原");
            switch (msg.ServiceID)
            {
                case 0:
                    agg.OperationTypeCode = EnmOperationType.RestoreCommonConfig;
                    sb.Append("公共");
                    break;
                case TdbCst.ServerID.Account:
                    agg.OperationTypeCode = EnmOperationType.RestoreAccountConfig;
                    sb.Append("账户服务");
                    break;
                case TdbCst.ServerID.Files:
                    agg.OperationTypeCode = EnmOperationType.RestoreFilesConfig;
                    sb.Append("文件服务");
                    break;
                case TdbCst.ServerID.Admin:
                    agg.OperationTypeCode = EnmOperationType.RestoreAdminConfig;
                    sb.Append("运维服务");
                    break;
                case TdbCst.ServerID.Relationships:
                    agg.OperationTypeCode = EnmOperationType.RestoreRelationshipsConfig;
                    sb.Append("人际关系服务");
                    break;
                default:
                    throw new TdbException($"还原配置通知接收到未知服务类型：{msg.ServiceID}");
            }
            sb.Append("分布式配置为：");

            var fileContent = File.ReadAllText(msg.JsonPath);
            sb.Append(fileContent);
            agg.Content = sb.ToString();

            //保存
            await agg.SaveAsync();
        }
    }
}
