using DotNetCore.CAP;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure.Services.Bus.CAP
{
    /// <summary>
    /// CAP
    /// </summary>
    public class TdbCAP
    {
        //Task PublishAsync<T>(string name, T? contentObj, string? callbackName = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T">内容对象类型</typeparam>
        /// <param name="topic">主题</param>
        /// <param name="contentObj">内容</param>
        /// <param name="callbackName">回调主题名称</param>
        public static void Publish<T>(string topic, T? contentObj, string? callbackName = null)
        {
            var capPublisher = GetCapPublisher();
            capPublisher.Publish(topic, contentObj, callbackName);
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
    }
}
