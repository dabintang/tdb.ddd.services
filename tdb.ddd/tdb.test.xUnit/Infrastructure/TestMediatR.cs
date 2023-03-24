using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using tdb.ddd.infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace tdb.test.xUnit.Infrastructure
{
    /// <summary>
    /// 测试MediatR
    /// </summary>
    public class TestMediatR
    {
        static string GuidValue = "";

        public TestMediatR()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTdbBusMediatR(() => new TdbBusAssemblyModule().GetRegisterAssemblys());
            var provider = services.BuildServiceProvider();

            TdbIOC.Init(provider);
        }

        /// <summary>
        /// 测试send方法
        /// </summary>
        [Fact]
        public async void TestSend()
        {
            var req = new AddRequest()
            {
                Value1 = 1,
                Value2 = 2
            };

            var res = await TdbMediatR.SendAsync(req);
            Assert.Equal(3M, res);
        }

        /// <summary>
        /// 测试Publish方法
        /// </summary>
        [Fact]
        public async void TestPublish()
        {
            var req = new SetNotification();

            await TdbMediatR.PublishAsync(req);
            Assert.Equal(GuidValue, req.GuidValue);
        }

        #region IRequest

        /// <summary>
        /// 加法请求
        /// </summary>
        class AddRequest : IRequest<decimal>
        {
            /// <summary>
            /// 第1个值
            /// </summary>
            public decimal Value1 { get; set; }

            /// <summary>
            /// 第2个值
            /// </summary>
            public decimal Value2 { get; set; }
        }

        /// <summary>
        /// 加法处理
        /// </summary>
        class AddRequestHandler : IRequestHandler<AddRequest, decimal>
        {
            /// <summary>
            /// 处理
            /// </summary>
            /// <param name="request"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public async Task<decimal> Handle(AddRequest request, CancellationToken cancellationToken)
            {
                await Task.CompletedTask;
                return request.Value1 + request.Value2;
            }
        }

        #endregion

        #region INotification

        /// <summary>
        /// 设置Guid缓存 通知
        /// </summary>
        public class SetNotification : INotification
        {
            /// <summary>
            /// Guid
            /// </summary>
            public string GuidValue { get; set; } = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 设置Guid缓存处理
        /// </summary>
        public class SetNotificationHandler : INotificationHandler<SetNotification>
        {
            /// <summary>
            /// 设置Guid缓存处理
            /// </summary>
            /// <param name="notification"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public async Task Handle(SetNotification notification, CancellationToken cancellationToken)
            {
                await Task.CompletedTask;
                GuidValue = notification.GuidValue;
            }
        }

        #endregion
    }
}
