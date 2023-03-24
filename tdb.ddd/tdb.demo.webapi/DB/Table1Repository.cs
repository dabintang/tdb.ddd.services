using SqlSugar;
using System.Diagnostics;
using System.Reflection;
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
            string text = "";
            StackTrace stackTrace = new StackTrace(fNeedFileInfo: true);
            StackFrame[] frames = stackTrace.GetFrames();
            bool flag = UtilMethods.IsAnyAsyncMethod(frames);
            if (Task.CurrentId.HasValue)
            {
                StackFrame[] array = frames;
                foreach (StackFrame stackFrame in array)
                {
                    MethodBase? method = stackFrame?.GetMethod();
                    if (method?.Name == "MoveNext" && method.ReflectedType!.FullName!.StartsWith("Quartz."))
                    {
                        text += "Quartz";
                        break;
                    }
                }
            }
            TdbLogger.Ins.Debug($"text：{text}");

            TdbLogger.Ins.Debug($"Table1Repository：{this.Context.ContextID}");
            return this.Context.ContextID;
        }
    }
}
