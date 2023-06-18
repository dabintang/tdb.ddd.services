using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.domain.OperationRecord;
using tdb.ddd.admin.domain.OperationRecord.Aggregate;
using tdb.ddd.admin.repository.DBEntity;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.admin.repository
{
    /// <summary>
    /// 操作记录仓储
    /// </summary>
    public class OperationRecordRepos : TdbRepository<OperationRecord>, IOperationRecordRepos
    {
        #region 实现接口

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="agg">操作记录聚合</param>
        public async Task SaveAsync(OperationRecordAgg agg)
        {
            //转换为数据库实体
            var info = DBMapper.Map<OperationRecordAgg, OperationRecord>(agg);

            //保存
            await this.InsertOrUpdateAsync(info);
        }

        #endregion
    }
}
