using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.repository.sqlsugar
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public static class TdbDBContext
    {
        /// <summary>
        /// 获取数据库上下文
        /// </summary>
        /// <returns></returns>
        public static ISqlSugarClient GetDBContext()
        {
            return DbScoped.SugarScope.ScopedContext;
        }
    }
}
