using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.webapi;
using tdb.demo.webapi.Configs;

namespace tdb.demo.webapi.Controllers.V1
{
    /// <summary>
    /// CAP
    /// </summary>
    [TdbApiVersion(1)]
    [AllowAnonymous]
    public class CAPController : BaseController
    {
        #region 接口

        #region test.time.ok

        private const string topicOK = "test.time.ok";
        private const string callbackOK = "test.time.ok.callback";

        /// <summary>
        /// 发布(ok)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<TdbRes<string>> PublishOK()
        {
            var msg = new MsgTimeInfo();
            var header = this.GetCAPHeader("发布(ok)", callbackOK);
            await TdbCAP.PublishAsync(topicOK, msg, header);

            return TdbRes.Success(msg.ToString());
        }

        /// <summary>
        /// 订阅(ok)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(topicOK)]
        public TdbRes<MsgTimeInfo> SubscribeOK(MsgTimeInfo msg)
        {
            TdbLogger.Ins.Debug($"接收到消息[ok]：{msg}");

            return TdbRes.Success(msg);
        }

        /// <summary>
        /// 回调(ok)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(callbackOK)]
        public TdbRes<bool> CallbackOK(TdbRes<MsgTimeInfo> res)
        {
            TdbLogger.Ins.Debug($"接收到消息[callback.ok]：{res.SerializeJson()}");

            return TdbRes.Success(true);
        }

        #endregion

        #region test.time.error

        private const string topicError = "test.time.error";
        private const string callbackError = "test.time.error.callback";

        /// <summary>
        /// 发布(error)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<TdbRes<string>> PublishError()
        {
            var msg = new MsgTimeInfo();
            var header = this.GetCAPHeader("发布(error)", callbackError);
            await TdbCAP.PublishAsync(topicError, msg, header);

            return TdbRes.Success(msg.ToString());
        }

        /// <summary>
        /// 订阅(error)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(topicError)]
        public TdbRes<MsgTimeInfo> SubscribeError(MsgTimeInfo msg)
        {
            TdbLogger.Ins.Debug($"接收到消息[error]：{msg}。但处理异常");

            var num = new Random().Next(0, 100);
            if (num < 70)
            {
                throw new TdbException("cap消息处理异常");
            }

            return TdbRes.Success(msg);
        }

        /// <summary>
        /// 回调(error)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(callbackError)]
        public TdbRes<bool> CallbackError(TdbRes<MsgTimeInfo> res)
        {
            TdbLogger.Ins.Debug($"接收到消息[callback.error]：{res.SerializeJson()}");

            return TdbRes.Success(true);
        }

        #endregion

        #region test.time.longtime

        private const string topicLongTime = "test.time.longtime";
        private const string callbackLongTime = "test.time.longtime.callback";

        /// <summary>
        /// 发布(longtime)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<TdbRes<string>> PublishLongTime()
        {
            var msg = new MsgTimeInfo();
            var header = this.GetCAPHeader("发布(longtime)", callbackLongTime);
            await TdbCAP.PublishAsync(topicLongTime, msg, header);

            return TdbRes.Success(msg.ToString());
        }

        /// <summary>
        /// 订阅(longtime)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(topicLongTime)]
        public TdbRes<MsgTimeInfo> SubscribeLongTime(MsgTimeInfo msg)
        {
            TdbLogger.Ins.Debug($"接收到消息[longtime]：{msg}");

            Thread.Sleep(1000);

            return TdbRes.Success(msg);
        }

        /// <summary>
        /// 回调(longtime)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(callbackLongTime)]
        public TdbRes<bool> CallbackLongTime(TdbRes<MsgTimeInfo> res)
        {
            TdbLogger.Ins.Debug($"接收到消息[callback.longtime]：{res.SerializeJson()}");

            return TdbRes.Success(true);
        }

        #endregion

        #region test.time.group

        private const string topicGroup = "test.time.group";
        private const string callbackGroup = "test.time.group.callback";

        /// <summary>
        /// 发布(group)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<TdbRes<string>> PublishGroup()
        {
            var msg = new MsgTimeInfo();
            var header = this.GetCAPHeader("发布(group)", callbackGroup);
            await TdbCAP.PublishAsync(topicGroup, msg, header);

            return TdbRes.Success(msg.ToString());
        }

        /// <summary>
        /// 订阅(group)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(topicGroup, Group = "test")]
        public TdbRes<MsgTimeInfo> SubscribeGroup(MsgTimeInfo msg)
        {
            TdbLogger.Ins.Debug($"接收到消息[group]：{msg}");

            return TdbRes.Success(msg);
        }

        /// <summary>
        /// 回调(group)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(callbackGroup,  Group = "test")]
        public TdbRes<bool> CallbackGroup(TdbRes<MsgTimeInfo> res)
        {
            TdbLogger.Ins.Debug($"接收到消息[callback.group]：{res.SerializeJson()}");

            return TdbRes.Success(true);
        }

        #endregion

        /// <summary>
        /// 发布大量(ok)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<TdbRes<int>> PublishOKLot([FromQuery]int count)
        {
            count = Math.Max(count, 100);

            for (int i = 0; i < count; i++)
            {
                var msg = new MsgTimeInfo();
                var header = this.GetCAPHeader("发布大量(ok)", null);
                await TdbCAP.PublishAsync(topicOK, msg, header);
                //await TdbCAP.PublishAsync(topicLongTime, msg, header);
                //await TdbCAP.PublishAsync(topicLongTime, msg, header);
            }

            return TdbRes.Success(count);
        }

        /// <summary>
        /// 获取cap头部信息
        /// </summary>
        /// <param name="triggerEventDesc">触发事件描述</param>
        /// <param name="callbackName">回调主题名称</param>
        /// <returns></returns>
        private Dictionary<string, string> GetCAPHeader(string triggerEventDesc, string callbackName)
        {
            return new Dictionary<string, string>()
            {
                { TdbCst.CAPHeaders.ServerID, DemoConfig.App.Server.ID.ToStr() },
                { TdbCst.CAPHeaders.ServerSeq, DemoConfig.App.Server.Seq.ToStr() },
                { TdbCst.CAPHeaders.Source, triggerEventDesc },
                { Headers.CallbackName, callbackName }
            };
        }

        #endregion

        #region 定义

        /// <summary>
        /// 时间消息
        /// </summary>
        public class MsgTimeInfo
        {
            /// <summary>
            /// guid no
            /// </summary>
            public string GuidNO { get; set; } = Guid.NewGuid().ToString();

            /// <summary>
            /// 当前时间
            /// </summary>
            public DateTime Now { get; set; } = DateTime.Now;

            /// <summary>
            /// 转字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{this.GuidNO}.{this.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}";
            }
        }

        #endregion
    }
}
