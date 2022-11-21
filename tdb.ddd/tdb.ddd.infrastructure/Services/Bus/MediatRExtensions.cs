using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.infrastructure.Services.Bus
{
    /// <summary>
    /// MediatR扩展
    /// </summary>
    public static class MediatRExtensions
    {
        /// <summary>
        /// 无等待通知
        /// </summary>
        /// <typeparam name="TNotification">通知消息类型</typeparam>
        /// <param name="mediator"></param>
        /// <param name="notification">通知消息</param>
        /// <param name="cancellationToken"></param>
        public static void PublishNotWait<TNotification>(this IMediator mediator, TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            mediator.Publish<TNotification>(notification, cancellationToken);
        }
    }
}
