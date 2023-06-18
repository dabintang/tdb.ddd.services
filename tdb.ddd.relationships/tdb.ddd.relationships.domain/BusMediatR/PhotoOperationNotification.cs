using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.relationships.domain.BusMediatR
{
    /// <summary>
    /// 照片操作通知消息
    /// </summary>
    public class PhotoOperationNotification : INotification
    {
        /// <summary>
        /// 照片ID
        /// </summary>
        public long PhotoID { get; set; }

        /// <summary>
        /// 操作类型（1：保存；2：删除）
        /// </summary>
        public EnmOperationType OperationTypeCode { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public long OperatorID { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 操作类型（1：保存；2：删除）
        /// </summary>
        public enum EnmOperationType
        {
            /// <summary>
            /// 1：保存
            /// </summary>
            Save = 1,

            /// <summary>
            /// 2：删除
            /// </summary>
            Delete = 2
        }
    }
}
