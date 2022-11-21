using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.repository.sqlsugar
{
    /// <summary>
    /// 仓储事务
    /// </summary>
    public class TdbRepositoryTran
    {
        /// <summary>
        /// 开始事务
        /// </summary>
        public static void BeginTran()
        {
            TdbDbScoped.GetScopedContext().AsTenant().BeginTran();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public static void CommitTran()
        {
            TdbDbScoped.GetScopedContext().AsTenant().CommitTran();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public static void RollbackTran()
        {
            TdbDbScoped.GetScopedContext().AsTenant().RollbackTran();
        }
    }
}
