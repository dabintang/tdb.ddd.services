using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 全局异常捕获过滤器
    /// </summary>
    public class TdbGlobalExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// 进行异常转码返回、并记录日志
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            //异常是否已处理
            if (context.ExceptionHandled == false)
            {
                //内部已知异常
                if (context.Exception is TdbException tdbEx)
                {
                    //返回比较友好的错误信息
                    context.Result = new ObjectResult(new TdbRes<object>(TdbComResMsg.Fail.FromNewMsg(tdbEx.Message), null));
                }
                else
                {
                    //返回比较友好的错误信息
                    context.Result = new Microsoft.AspNetCore.Mvc.ContentResult
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Content = "服务器忙",
                        ContentType = "text/html;charset=utf-8"
                    };
                }

                //写日志
                try
                {
                    TdbLogger.Ins.Error(context.Exception, context.Exception.Message);
                }
                catch
                {
                    Console.WriteLine($"[GlobalExceptionFilter.OnException]未加载日志服务，请先在IServiceCollection添加日志服务。ex：{context.Exception}");
                }

                context.ExceptionHandled = true; //标记异常为已处理
            }
        }
    }
}
