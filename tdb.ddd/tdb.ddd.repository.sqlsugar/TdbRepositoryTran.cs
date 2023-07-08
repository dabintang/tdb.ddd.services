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
        /// 开始事务，可保证从调用位置开始至方法结束期间内用的数据库上下文为同一个。
        /// （注：需在[异步方法]上调用才会起效）
        /// </summary>
        public static void BeginTranOnAsyncFunc()
        {
            DbScoped.SugarScope.ScopedContext.AsTenant().BeginTran();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public static void CommitTran()
        {
            DbScoped.SugarScope.ScopedContext.AsTenant().CommitTran();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public static void RollbackTran()
        {
            DbScoped.SugarScope.ScopedContext.AsTenant().RollbackTran();
        }
    }
}
