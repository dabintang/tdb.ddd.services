using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// MediatR
    /// </summary>
    public class TdbMediatR
    {
        /// <summary>
        /// 无等待通知
        /// </summary>
        /// <typeparam name="TNotification">通知消息类型</typeparam>
        /// <param name="notification">通知消息</param>
        /// <param name="cancellationToken"></param>
        public static void Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            var mediator = GetMediator();
            mediator.Publish<TNotification>(notification, cancellationToken);
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <typeparam name="TNotification">通知消息类型</typeparam>
        /// <param name="notification">通知消息</param>
        /// <param name="cancellationToken"></param>
        public static async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            var mediator = GetMediator();
            await mediator.Publish<TNotification>(notification, cancellationToken);
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <typeparam name="TResponse">请求参数类型</typeparam>
        /// <param name="request">请求参数</param>
        /// <param name="cancellationToken"></param>
        public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var mediator = GetMediator();
            return await mediator.Send<TResponse>(request, cancellationToken);
        }

        /// <summary>
        /// 获取IMediator
        /// </summary>
        /// <returns></returns>
        private static IMediator GetMediator()
        {
            var mediator = TdbIOC.GetService<IMediator>();
            if (mediator == null)
            {
                throw new TdbException("未配置MediatR，请先调用方法IServiceCollection.AddTdbBusMediatR");
            }

            return mediator;
        }
    }
}
