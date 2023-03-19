using DotNetCore.CAP.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// cap订阅过滤器
    /// </summary>
    public class TdbCAPSubscribeFilter : SubscribeFilter
    {
        /// <summary>
        /// 订阅方法执行前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnSubscribeExecutingAsync(ExecutingContext context)
        {
            TryWriteLog($"收到CAP消息：{context.DeliverMessage.SerializeJson()}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 订阅方法执行后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnSubscribeExecutedAsync(ExecutedContext context)
        {
            TryWriteLog($"处理CAP消息完成：{context.DeliverMessage.SerializeJson()}；结果：{context.Result.SerializeJson()}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 订阅方法执行异常
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnSubscribeExceptionAsync(ExceptionContext context)
        {
            TryWriteLog($"处理CAP消息异常：{context.DeliverMessage.SerializeJson()}；异常：{context.Exception.ToString()}", EnmTdbLogLevel.Error);
            return Task.CompletedTask;
        }

        #region 私有方法

        /// <summary>
        /// 尝试写日志
        /// </summary>
        /// <param name="msg">日志消息</param>
        /// <param name="level">日志级别</param>
        private static void TryWriteLog(string msg, EnmTdbLogLevel level = EnmTdbLogLevel.Trace)
        {
            var logger = TdbIOC.GetService<ITdbLog>();
            if (logger is null)
            {
                return;
            }

            //写日志
            logger.Log(level, msg);
        }

        #endregion
    }
}
