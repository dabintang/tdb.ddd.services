using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// CAP
    /// </summary>
    public class TdbCAP
    {
        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T">内容对象类型</typeparam>
        /// <param name="topic">主题名称</param>
        /// <param name="contentObj">消息内容</param>
        /// <param name="callbackName">回调主题名称</param>
        public static async Task PublishAsync<T>(string topic, T? contentObj, string? callbackName = null)
        {
            var capPublisher = GetCapPublisher();

            var headers = new Dictionary<string, string?>();
            if (string.IsNullOrWhiteSpace(callbackName) == false)
            {
                headers[Headers.CallbackName] = callbackName;
            }

            await PublishAsync(topic, contentObj, headers);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T">内容对象类型</typeparam>
        /// <param name="topic">主题名称</param>
        /// <param name="contentObj">消息内容</param>
        /// <param name="headers">消息附件头部信息</param>
        public static async Task PublishAsync<T>(string topic, T? contentObj, IDictionary<string, string?> headers)
        {
            var capPublisher = GetCapPublisher();
            await capPublisher.PublishAsync(topic, contentObj, headers);

            TryWriteLog($"发布CAP消息：{contentObj.SerializeJson()}；headers:{headers.SerializeJson()}");
        }

        /// <summary>
        /// 获取ICapPublisher
        /// </summary>
        /// <returns></returns>
        private static ICapPublisher GetCapPublisher()
        {
            var capPublisher = TdbIOC.GetService<ICapPublisher>();
            if (capPublisher == null)
            {
                throw new TdbException("未配置CAP，请先调用方法IServiceCollection.AddTdbBusCAP");
            }

            return capPublisher;
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
