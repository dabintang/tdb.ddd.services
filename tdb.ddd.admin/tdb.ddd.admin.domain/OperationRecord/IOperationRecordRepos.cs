using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.domain.OperationRecord.Aggregate;
using tdb.ddd.contracts;

namespace tdb.ddd.admin.domain.OperationRecord
{
    /// <summary>
    /// 操作记录仓储接口
    /// </summary>
    public interface IOperationRecordRepos : ITdbIOCScoped
    {
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">操作记录聚合</param>
        Task SaveAsync(OperationRecordAgg agg);
    }
}
