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
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long ID { get; set; }

        /// <summary>
        /// 操作类型编号
        /// </summary>
        public short OperationTypeCode { get; set; }

        /// <summary>
        /// 版本号（同一操作不能版本，内容结构可能不一样）
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        [SugarColumn(IsJson = true)]
        public JsonElement Content { get; set; }

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
