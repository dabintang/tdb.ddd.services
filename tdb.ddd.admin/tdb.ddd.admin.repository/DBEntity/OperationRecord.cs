using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace tdb.ddd.admin.repository.DBEntity
{
    /// <summary>
    /// 操作记录表
    /// </summary>
    [SugarTable("operation_record")]
    public class OperationRecord
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 操作类型编号
        /// </summary>
        public short OperationTypeCode { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long OperatorID { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }
    }
}
