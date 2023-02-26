using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using tdb.common;
using System.Net.Http.Json;
using System.Text.Json;
using tdb.ddd.infrastructure.Common.Http;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Reflection.PortableExecutable;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// http调用帮助类
    /// </summary>
    public class TdbHttpClient
    {
        /// <summary>
        /// 默认名
        /// </summary>
        public const string DefaultName = "tdb.httpclient.default";

        /// <summary>
        /// get请求
        /// </summary>
        /// <typeparam name="ReturnT">返回值类型</typeparam>
        /// <param name="requestUri">请求地址字符串</param>
        /// <param name="reqParam">请求参数</param>
        /// <param name="authentication">身份认证</param>
        /// <returns></returns>
        public static async Task<ReturnT?> GetAsync<ReturnT>([StringSyntax(StringSyntaxAttribute.Uri)] string requestUri, object? reqParam = null, AuthenticationHeaderValue? authentication = null)
        {
            //生成Flurl.Url
            var url = new TdbUrl(requestUri);
            
            //把请求条件合并到url上
            url.SetQueryParams(reqParam);

            //请求消息体
            var reqMsg = new HttpRequestMessage(HttpMethod.Get, url.ToUri());

            //添加身份认证信息
            if (authentication is not null)
            {
                reqMsg.Headers.Authorization = authentication;
            }

            //发送请求
            var response = await SendAsync(reqMsg);

            //从响应消息获取数据
            return await ReadData<ReturnT>(response);
        }

        /// <summary>
        /// get下载文件
        /// </summary>
        /// <typeparam name="RequestT">请求参数类型</typeparam>
        /// <typeparam name="ReturnT">返回值类型</typeparam>
        /// <param name="requestUri">请求地址字符串</param>
        /// <param name="reqParam">请求参数</param>
        /// <param name="authentication">身份认证</param>
        /// <returns></returns>
        public static async Task<TdbDownloadFileRes> DownloadFileAsync<RequestT, ReturnT>([StringSyntax(StringSyntaxAttribute.Uri)] string requestUri, RequestT? reqParam, AuthenticationHeaderValue? authentication = null)
        {
            //生成Flurl.Url
            var url = new TdbUrl(requestUri);

            //把请求条件合并到url上
            url.SetQueryParams(reqParam);

            //请求消息体
            var reqMsg = new HttpRequestMessage(HttpMethod.Get, url.ToUri());

            //添加身份认证信息
            if (authentication is not null)
            {
                reqMsg.Headers.Authorization = authentication;
            }

            //发送请求
            var response = await SendAsync(reqMsg, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);

            //断言响应消息状态为成功
            AssertResponseIsSuccess(response);

            //响应内容日志
            TryWriteLog(HttpResponseMessageToStr(response));

            //以流形式读取响应内容
            var contentStream = await response.Content.ReadAsStreamAsync();
            var result = new TdbDownloadFileRes()
            {
                FileName = response.Content.Headers.ContentDisposition?.FileName ?? "",
                ContentType = response.Content.Headers.ContentType?.ToStr() ?? "",
                ContentLength = response.Content.Headers.ContentLength,
                ContentStream = contentStream
            };

            return result;
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <typeparam name="RequestT">请求参数类型</typeparam>
        /// <typeparam name="ReturnT">返回值类型</typeparam>
        /// <param name="requestUri">请求地址字符串</param>
        /// <param name="reqParam">请求参数</param>
        /// <param name="options">json序列化选项</param>
        /// <param name="authentication">身份认证</param>
        /// <returns></returns>
        public static async Task<ReturnT?> PostAsJsonAsync<RequestT, ReturnT>([StringSyntax(StringSyntaxAttribute.Uri)] string requestUri, RequestT? reqParam, JsonSerializerOptions? options = null, AuthenticationHeaderValue? authentication = null)
        {
            //请求消息体
            var reqMsg = new HttpRequestMessage(HttpMethod.Post, requestUri);

            //添加身份认证信息
            if (authentication is not null)
            {
                reqMsg.Headers.Authorization = authentication;
            }

            //json内容
            options ??= CvtHelper.DefaultOptions;
            var content = JsonContent.Create(reqParam, mediaType: null, options: options);
            reqMsg.Content = content;

            //发送请求
            var response = await SendAsync(reqMsg);

            //从响应消息获取数据
            return await ReadData<ReturnT>(response);
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <typeparam name="ReturnT">返回值类型</typeparam>
        /// <param name="requestUri">请求地址字符串</param>
        /// <param name="reqParam">请求参数</param>
        /// <param name="options">json序列化选项</param>
        /// <param name="authentication">身份认证</param>
        /// <returns></returns>
        public static async Task<ReturnT?> PostAsJsonAsync<ReturnT>([StringSyntax(StringSyntaxAttribute.Uri)] string requestUri, object? reqParam, JsonSerializerOptions? options = null, AuthenticationHeaderValue? authentication = null)
        {
            //请求消息体
            var reqMsg = new HttpRequestMessage(HttpMethod.Post, requestUri);

            //添加身份认证信息
            if (authentication is not null)
            {
                reqMsg.Headers.Authorization = authentication;
            }

            //json内容
            options ??= CvtHelper.DefaultOptions;
            var content = JsonContent.Create(reqParam, mediaType: null, options: options);
            reqMsg.Content = content;

            //发送请求
            var response = await SendAsync(reqMsg);

            //从响应消息获取数据
            return await ReadData<ReturnT>(response);
        }

        /// <summary>
        /// post上传文件
        /// </summary>
        /// <typeparam name="RequestT">请求参数类型</typeparam>
        /// <typeparam name="ReturnT">返回值类型</typeparam>
        /// <param name="requestUri">请求地址字符串</param>
        /// <param name="stream">文件流</param>
        /// <param name="fileName">文件名</param>
        /// <param name="reqParam">请求参数</param>
        /// <param name="authentication">身份认证</param>
        /// <returns></returns>
        public static async Task<ReturnT?> UploadFileAsync<RequestT, ReturnT>([StringSyntax(StringSyntaxAttribute.Uri)] string requestUri, Stream stream, string fileName, RequestT? reqParam, AuthenticationHeaderValue? authentication = null)
        {
            //请求消息体
            var reqMsg = new HttpRequestMessage(HttpMethod.Post, requestUri);

            //添加身份认证信息
            if (authentication is not null)
            {
                reqMsg.Headers.Authorization = authentication;
            }

            //内容
            var content = new MultipartFormDataContent();
            //请求参数
            var lstKeyValue = TdbUrl.ToKeyValuePairs(reqParam);
            foreach (var (Key, Value) in lstKeyValue)
            {
                content.Add(new StringContent(Value?.ToStr() ?? ""), Key);
            }
            //文件
            using var streamContent = new StreamContent(stream, 3 * 1024);
            content.Add(streamContent, "file", fileName);
            reqMsg.Content = content;

            //发送请求
            var response = await SendAsync(reqMsg);

            //从响应消息获取数据
            return await ReadData<ReturnT>(response);
        }

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            //发送请求
            return await SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
        }

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <param name="completionOption">完成选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            //请求条件日志
            TryWriteLog($"发起http请求，条件={HttpRequestMessageToStr(request)}");

            //用IHttpClientFactory创建http客户端
            var client = CreateClient();
            //发送请求
            return await client.SendAsync(request, completionOption, cancellationToken);
        }

        #region 私有方法

        /// <summary>
        /// 用IHttpClientFactory创建http客户端
        /// </summary>
        /// <returns></returns>
        private static HttpClient CreateClient()
        {
            var factory = TdbIOC.GetService<IHttpClientFactory>();
            if (factory is null)
            {
                throw new TdbException("未注册IHttpClientFactory，请先调用方法IServiceCollection.AddHttpClient");
            }

            return factory.CreateClient(DefaultName);
        }

        /// <summary>
        /// 从响应消息获取数据
        /// </summary>
        /// <typeparam name="ReturnT">返回类型</typeparam>
        /// <param name="response">响应消息</param>
        /// <returns></returns>
        private static async Task<ReturnT?> ReadData<ReturnT>(HttpResponseMessage response)
        {
            //断言响应消息状态为成功
            AssertResponseIsSuccess(response);

            //返回信息转字符串
            var strContent = await response.Content.ReadAsStringAsync();

            //日志
            TryWriteLog($"http请求响应：{strContent}");

            return strContent.DeserializeJson<ReturnT>();
        }

        /// <summary>
        /// 断言响应消息状态为成功
        /// </summary>
        /// <param name="response">响应消息</param>
        /// <returns></returns>
        /// <exception cref="TdbException"></exception>
        private static void AssertResponseIsSuccess(HttpResponseMessage response)
        {
            //返回信息转字符串
            if (response.IsSuccessStatusCode == false)
            {
                throw new TdbException($"http请求异常：{HttpResponseMessageToStr(response)}");
            }
        }

        /// <summary>
        /// 尝试写日志（Trace级别）
        /// </summary>
        /// <param name="msg">日志消息</param>
        private static void TryWriteLog(string msg)
        {
            var logger = TdbIOC.GetService<ITdbLog>();
            if (logger is null)
            {
                return;
            }

            //写Trace级别日志
            logger.Trace(msg);
        }

        /// <summary>
        /// http请求消息转字符串
        /// </summary>
        /// <param name="request">http请求消息</param>
        /// <returns></returns>
        private static string HttpRequestMessageToStr(HttpRequestMessage request)
        {
            if (request.Content is JsonContent jsonContent)
            {
                var sb = new StringBuilder();

                sb.Append($"Method: {request.Method}");

                sb.Append($", RequestUri: '");
                sb.Append(request.RequestUri == null ? "<null>" : request.RequestUri.ToString());

                sb.Append($"', Version: {request.Version}");

                sb.Append($", Content: {jsonContent.SerializeJson()}");

                sb.Append($", Headers:{request.Headers.ToString()}");

                return sb.ToString();
            }
            else
            {
                return request.ToString();
            }
        }

        /// <summary>
        /// http响应消息转字符串
        /// </summary>
        /// <param name="response">http响应消息</param>
        /// <returns></returns>
        private static string HttpResponseMessageToStr(HttpResponseMessage response)
        {
            if (response.Content is JsonContent jsonContent)
            {
                var sb = new StringBuilder();

                sb.Append($"StatusCode: {(int)response.StatusCode}");

                sb.Append(", ReasonPhrase: '");
                sb.Append(response.ReasonPhrase ?? "<null>");

                sb.Append($"', Version: {response.Version}");

                sb.Append($", Content: {jsonContent.SerializeJson()}");

                sb.AppendLine($", Headers: {{response.Headers.ToString()}}");

                return sb.ToString();
            }
            else
            {
                return response.ToString();
            }
        }

        #endregion
    }
}
