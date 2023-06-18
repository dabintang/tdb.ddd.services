using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.domain.contracts.Enum;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.ddd.admin.domain.OperationRecord.Aggregate
{
    /// <summary>
    /// 操作记录聚合
    /// </summary>
    public class OperationRecordAgg : TdbAggregateRoot<long>
    {
        #region 仓储

        private IOperationRecordRepos? _operationRecordRepos;
        /// <summary>
        /// 操作记录仓储
        /// </summary>
        private IOperationRecordRepos OperationRecordRepos
        {
            get
            {
                this._operationRecordRepos ??= TdbIOC.GetService<IOperationRecordRepos>();
                if (this._operationRecordRepos is null)
                {
                    throw new TdbException("操作记录仓储接口未实现");
                }

                return this._operationRecordRepos;
            }
        }

        #endregion

        #region 值

        /// <summary>
        /// 操作类型编号
        /// </summary>
        public EnmOperationType OperationTypeCode { get; set; }

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

        #endregion

        #region 行为

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await this.OperationRecordRepos.SaveAsync(this);
        }

        #endregion
    }
}
