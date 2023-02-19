//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using tdb.ddd.contracts;

//namespace tdb.ddd.infrastructure
//{
//    /// <summary>
//    /// http请求日志处理
//    /// </summary>
//    public class TdbHttpClientLogHandler : DelegatingHandler, ITdbIOCTransient
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="request"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
//        {
//            //请求条件日志
//            TryWriteLog($"http请求开始，条件={request}");

//            var response = base.Send(request, cancellationToken);

//            //请求结果日志
//            TryWriteLog($"http请求完成，结果={response}");

//            return response;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="request"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//        {
//            //请求条件日志
//            TryWriteLog($"http请求开始，条件={request}");

//            var response = await base.SendAsync(request, cancellationToken);

//            //响应数据大小（字节）
//            var contentLength = response.Content.Headers.ContentLength;
//            //响应内容字符串
//            var sbLogResponse = new StringBuilder();
//            if (contentLength.HasValue)
//            {
//                sbLogResponse.Append($"长度[{contentLength}]字节");

//                //如果小于10k，则把内容打印处理
//                if (contentLength < 1024 * 10)
//                {
//                    sbLogResponse.Append($"、内容[{await response.Content.ReadAsStringAsync()}]");
//                }
//            }

//            //请求结果日志
//            if (response.IsSuccessStatusCode)
//            {
//                TryWriteLog($"http请求成功，{sbLogResponse}");
//            }
//            else
//            {
//                TryWriteLog($"http请求失败，{sbLogResponse}");
//            }

//            return response;
//        }

//        #region 私有方法

//        /// <summary>
//        /// 尝试写日志（Trace级别）
//        /// </summary>
//        /// <param name="msg">日志消息</param>
//        private static void TryWriteLog(string msg)
//        {
//            var logger = TdbIOC.GetService<ITdbLog>();
//            if (logger is null)
//            {
//                return;
//            }

//            //写Trace级别日志
//            logger.Trace(msg);
//        }

//        #endregion
//    }
//}
