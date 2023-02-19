using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Text.Json;
using tdb.common;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// api调用日志
    /// </summary>
    public class TdbAPILogAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 日志级别（默认：Info）
        /// </summary>
        public EnmTdbLogLevel Level { get; set; } = EnmTdbLogLevel.Info;

        /// <summary>
        /// 进入接口
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                //是否支持指定的日志级别
                if (TdbLogger.Ins.IsEnabled(this.Level))
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"进入接口[{context.ActionDescriptor.DisplayName}]，UID={GetUID(context.HttpContext)}");
                    sb.AppendLine($"入参：{context.ActionArguments.SerializeJson()}");
                    TdbLogger.Ins.Log(this.Level, sb.ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                TdbLogger.Ins.Error(ex, "进入接口写日志异常");
            }

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 离开接口
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);

            try
            {
                //是否支持指定的日志级别
                if (TdbLogger.Ins.IsEnabled(this.Level))
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"离开接口[{context.ActionDescriptor.DisplayName}]，UID={GetUID(context.HttpContext)}");

                    if (context.Exception == null)
                    {
                        sb.Append("出参：");

                        if (context.Result is ObjectResult objReulst)
                        {
                            sb.Append(objReulst.Value.SerializeJson());
                        }
                        else if (context.Result is FileResult)
                        {
                            sb.Append("文件流");
                        }
                        else
                        {
                            if (CommHelper.IsExistPropertyOrField(context.Result, "Value"))
                            {
                                var value = CommHelper.ReflectGet(context.Result, "Value");
                                sb.Append(value.SerializeJson());
                            }
                            else
                            {
                                sb.Append(context.Result.SerializeJson());
                            }
                        }
                    }
                    else
                    {
                        sb.Append($"发生异常：{context.Exception}");
                    }

                    TdbLogger.Ins.Log(this.Level, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                TdbLogger.Ins.Error(ex, "离开接口写日志异常");
            }
        }

        /// <summary>
        /// 获取登录人编号
        /// </summary>
        /// <param name="context">http上下文</param>
        /// <returns></returns>
        private static string GetUID(HttpContext context)
        {
            //无认证用户
            if (context == null || context.User == null)
            {
                return "";
            }

            return context.User.FindFirst(TdbClaimTypes.UID)?.Value ?? "";
        }
    }
}
