using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.webapi;

namespace tdb.demo.webapi.DB
{
    /// <summary>
    /// table1仓储
    /// </summary>
    public class Table1Repository: TdbRepository<Table1>
    {
        /// <summary>
        /// 清空表
        /// </summary>
        public void Truncate()
        {
            this.Context.Ado.ExecuteCommand("TRUNCATE TABLE table1");
        }

        /// <summary>
        /// 获取数据库上下文ID
        /// </summary>
        public Guid GetContextID()
        {
            TdbLogger.Ins.Debug($"Table1Repository：{this.Context.ContextID}");
            return this.Context.ContextID;
        }
    }
}
