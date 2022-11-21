using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using tdb.common;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 参数验证过滤器
    /// </summary>
    public class TdbParamValidateFilter : IActionFilter
    {
        /// <summary>
        /// action执行前
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid && context.ModelState.ErrorCount > 0)
            {
                //获取第一个错误信息
                var errModelState = context.ModelState.FirstOrDefault(m => m.Value?.Errors.Count > 0);
                var strErr = errModelState.Value?.Errors.First().ErrorMessage ?? "未知错误";

                try
                {
                    var msgInfo = strErr.DeserializeJson<TdbInvalidParamInfo>();
                    context.Result = new ObjectResult(new TdbRes<object>(TdbComResMsg.InvalidParam.FromNewMsg(msgInfo?.Msg ?? "非法参数"), null));
                }
                catch
                {
                    var field = errModelState.Key;
                    var message = $"入参类型错误（字段：{field}）";
                    context.Result = new Microsoft.AspNetCore.Mvc.ContentResult
                    {
                        Content = message,
                        StatusCode = 400,
                        ContentType = "text/html;charset=utf-8"
                    };
                }
            }
        }

        /// <summary>
        /// action执行
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
