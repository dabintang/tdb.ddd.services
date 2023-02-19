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

//var httpClient = new HttpClient();
//var url = "http://localhost:9000/bigdata.mp4";
//var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
//var totalLength = response.Content.Headers.ContentLength;
//var contentStream = await response.Content.ReadAsStreamAsync();
//var fileStream = new FileStream("e:\\bigdata.db", FileMode.OpenOrCreate, FileAccess.Write);
//byte[] buffer = new byte[5 * 1024];//5KB缓存
//long readLength = 0L;
//int length;
//while ((length = await contentStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
//{
//    readLength += length;
//    if (totalLength > 0)
//    {
//        Console.WriteLine("下载进度: " + Math.Round((double)readLength / totalLength.Value * 100, 2) + "%");
//    }
//    else
//    {
//        Console.WriteLine("已下载: " + Math.Round((readLength / 1024.0), 2) + "KB");
//    }
//    fileStream.Write(buffer, 0, length);
//}
//fileStream.Close();






//window10
//vs2019
//.netcore 3.1
//centos 7.6
//一、在c#中发送http请求的方式
//本部分参考：《WebClient, HttpClient, HttpWebRequest , RestSharp之间的区别与抉择》

//在c#中常见发送http请求的方式如下：

//HttpWebRequest：

//.net 平台原生提供，这是.NET创建者最初开发用于使用HTTP请求的标准类。使用HttpWebRequest可以让开发者控制请求/响应流程的各个方面，如 timeouts, cookies, headers, protocols。
//.
//关于使用HttpWebRequest上传和下载文件，可参考：《c#使用Http上传下载文件》

//WebClient：

//.net 平台原生提供，WebClient是一种更高级别的抽象，是HttpWebRequest为了简化最常见任务而创建的，但也因此缺少了HttpWebRequest的灵活性。

//HttpClient：

//.net 平台原生提供，也是这次主讲的内容。

//RestSharp：

//开源项目，它是基于HttpWebRequest做的二次封装，这里不再说明，可参考：《c#: 使用restsharp发送http请求、下载文件》。

//Flurl：

//开源项目，基于HttpClient做的二次封装，项目地址：https://github.com/tmenier/Flurl

//在.net core平台下推荐使用HttpClient。

//二、HttpClient介绍
//本部分参考： 《MSDN: SocketsHttpHandler 》

//在System.Net.Http命名空间下的HttpClient是.net core平台最常用的http请求工具，它直接基于Socket开发，提供了异步友好的代码编写方式。

//下面是简单示例：

//看样子使用起来挺简单的。其实隐藏了一些细节，如果我们要对这些细节进行配置的话，建议写成下面的形式：

//// 在.net core 2.1之后，默认所有的http请求都会交给SocketsHttpHandler处理
//var socketsHttpHandler = new SocketsHttpHandler()
//{
//    AllowAutoRedirect = true,// 默认为true,是否允许重定向
//    MaxAutomaticRedirections = 50,//最多重定向几次,默认50次
//    //MaxConnectionsPerServer = 100,//连接池中统一TcpServer的最大连接数
//    UseCookies = false// 是否自动处理cookie
//};
//var client = new HttpClient(socketsHttpHandler);

//三、HttpClient相关的类
//HttpClient类：提供用户调用的入口；
//HttpRequestMessage类：表示用户请求消息；
//HttpResponseMessage类：表示http响应消息；
//上面是我们最常见的类，除此之外还有：

//HttpMessageInvoker：表示发起http消息的入口，HttpClient类就是继承了它，但也仅有HttpClient继承它；
//HttpMessageHandler：虽然HttpMessageInvoker表示http消息的入口，但发送http消息还要靠HttpMessageHandler，事实上，HttpMessageInvoker内部就封装者一个HttpMessageHandler；
//SocketsHttpHandler：继承HttpMessageHandler，它是.net core2.1之后事实上的HttpMessageHandler，也就是说我们代码中发送http消息基本用的就是它；
//HttpClientHandler：也继承自HttpMessageHandler，但其内部封装者SocketsHttpHandler，一般情况下http请求是转发给内部的SocketsHttpHandler处理的；
//DelegatingHandler：也继承自HttpMessageHandler，不过它是一个抽象类，旨在提供一个http请求管道的基类；
//四、HttpClient使用时的注意事项
//HttpClient类旨在提供一个用户入口，其内部管理着不同服务器的TCP连接池，如下图所示：


//所以，当我们需要发起http请求时，最好使用全局单例的HttpClient，而不是每次都new一个HttpClient。

//另外，由于TCP本身在断开连接的时候需要4次挥手动作，而其中又有一个等待时间，所以我们即使将HttpClient.Dispose()掉也会造成这个TCP连接短时间内无法断开（最长要持续4分钟），如果遇到高并发的话，很可能端口就不够用了。

//注意：上面缓存连接的时候是以传入的地址前缀做key，而不是最终解析的ip地址，所以，HttpClient对DNS解析不太友好。
//HttpClient是线程安全的，里面封装了链接池，使用DnSpy验证如下：


//五、HttpClient的使用配置
//当我们发送http请求时，我们需要关注一些事情，比如：

//是否自动处理cookie；

//默认HttpClient是自动处理cookie的，即：上一个请求返回的cookie，可能会随着下次请求发送出去。
//然而，最佳的使用方式是多次请求使用相同的HttpClient所以这个cookie隔离性就很差，我们可以在创建HttpClient的时候进行配置禁用cookie自动处理：

//var socketsHttpHandler = new SocketsHttpHandler()
//                         {
//                             UseCookies = false,// 是否自动处理cookie
//                         };
//var client = new HttpClient(socketsHttpHandler);

//是否自动重定向以及最多重定向几次；

//默认HttpClient自动处理重定向请求，并且最多重定向50次，一般我们不需要修改这个配置，但我们做测试的话，可以向下面写法：

//var socketsHttpHandler = new SocketsHttpHandler()
//{
//    AllowAutoRedirect = true,//是否自动重定向
//    MaxAutomaticRedirections = 50//自动重定向的最大次数
//};
//var client = new HttpClient(socketsHttpHandler);

//内部TCP链接池的设置；

//这个地方有三个配置项：

//MaxConnectionsPerServer: 每个url（如：http://www.baidu.com:80）最多有几个链接，默认是int.MaxValue。注意：url是不带路径及参数；
//PooledConnectionIdleTimeout: 每个TCP链接空闲的时间，因为TCP长时间不用也要及时释放嘛，此处默认2分钟；
//PooledConnectionLifetime: 每个TCP链接从创建开始存活的时间，默认是不限制的，一般也不用设置这个参数；
//直接看代码示例：

//var socketsHttpHandler = new SocketsHttpHandler()
//{
//    //每个请求连接的最大数量，默认是int.MaxValue,可以认为是不限制
//    MaxConnectionsPerServer = 100,
//    //连接池中TCP连接最多可以闲置多久,默认2分钟
//    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
//    //连接最长的存活时间,默认是不限制的,一般不用设置
//    PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
//};
//var client = new HttpClient(socketsHttpHandler);

//是否压缩；

//默认是不压缩，如果设置开启压缩的话，http请求头中会自动加上Accept - Encoding: gzip（当然你得设置压缩选项是gzip），如果后台也支持这种压缩的话，
//就会把消息体压缩并在响应头中添加Content - Encoding: gzip。asp.net core添加压缩支持参考：《ASP.NET Core中的响应压缩》
//设置的代码示例如下：

//var socketsHttpHandler = new SocketsHttpHandler()
//                         {
//                             //默认是None，即不压缩
//                             AutomaticDecompression = DecompressionMethods.GZip,
//                         };
//var client = new HttpClient(socketsHttpHandler);

//超时设置；

//有三个配置项：

//ConnectTimeout: 连接时超时时间，默认不限制
//Expect100ContinueTimeout: 等待返回100状态码的时间，默认1秒，根据msdn解释，当请求头有Expect: 100 - continue的时候，服务端应返回100状态码
//Timeout: 等待响应超时时间，默认：100秒。
//看下面的代码示例：

//var socketsHttpHandler = new SocketsHttpHandler()
//{
//    //建立TCP连接时的超时时间,默认不限制
//    ConnectTimeout = Timeout.InfiniteTimeSpan,
//    //等待服务返回statusCode=100的超时时间,默认1秒
//    Expect100ContinueTimeout = TimeSpan.FromSeconds(1),
//};
//var client = new HttpClient(socketsHttpHandler);
////等待响应超时时间，默认：100秒。
//client.Timeout = TimeSpan.FromSeconds(100);

//响应头数据大小限制；

//MaxResponseHeadersLength: http响应头最大字节数（单位：KB），默认：64，即：http响应头最大64KB，一般不用设置
//看代码设置：

//var socketsHttpHandler = new SocketsHttpHandler()
//{
//   MaxResponseHeadersLength = 64, //单位: KB
//};
//var client = new HttpClient(socketsHttpHandler);

//关于Drain的配置；

//没有搞懂什么是Drain，好像是：当关闭连接时需要从这个连接中排出未使用的数据，当排出超时或排出的字节数超出限制时就直接把连接关闭了，而不是放到池子里重用。对应的配置项：
//MaxResponseDrainSize、ResponseDrainTimeout。

//关于BaseAddress的配置：可以给HttpClient设置基地址，当HttpClient发送的请求不包含前缀时，将自动拼接上，否则不予拼接，如下：

//var client = new HttpClient();
//client.BaseAddress = new Uri("http://192.168.0.9:9000/");
//var url = "/index.html";
//await client.GetAsync(url);// 真实地址是： http://192.168.0.9:9000/index.html

//默认的http版本、请求头：

//默认的http版本是1.1，实验设置2.0没效果；
//默认请求头为空，可以自己设置如下：

//var client = new HttpClient();
//client.DefaultRequestVersion = HttpVersion.Version20;
//client.DefaultRequestHeaders.Add("machine-id", "1");

//关于SslOptions:

//可以在这里面配置SSL相关的东西。这里我只实验了一种场景，即：访问https网站时由于网站自身的证书不规范导致报错：“AuthenticationException: The remote certificate is invalid according to the validation procedure.”
//在我们使用HttpWebRequest的时候我们可以通过下面回调设置：
//httpWebRequest.ServerCertificateValidationCallback = (sender, cer, chain, err) => true;
//在HttpClient时，我们对应的设置为：

//var socketsHandler = new SocketsHttpHandler()
//{
//    SslOptions = new System.Net.Security.SslClientAuthenticationOptions()
//    {
//          RemoteCertificateValidationCallback = (sender, cer, chain, err) => true                    
//    }
// };
// var httpClient = new HttpClient(socketsHandler);

//需要注意： 这是由于网站自身的证书不规范造成的，我们正常访问 https://www.baidu.com 即使不加这个配置也是正常的。

//验证相关：PreAuthenticate、Credentials，不过这两个怎么实验都看不到效果，应该也用不到。
//代理相关：Proxy、UseProxy、DefaultProxyCredentials 这几个未做实验。

//六、HttpClient提供的方法
//6.1 通过Get请求数据
//var httpClient = new HttpClient();
//var url = "http://localhost:9000/index.html";
//var response = await httpClient.GetAsync(url);
//var str = await response.Content.ReadAsStringAsync();

//6.2 通过Get下载文件
//现在我们要下载一个文件，假如这个文件不超过2G且不需要下载进度提示，那么我们可以如下操作：

//  var httpClient = new HttpClient();
//  var response = await httpClient.GetAsync("http://localhost:9000/middledata.mp4");//763M
//  var fileStream = new FileStream("e:\\middle.db", FileMode.OpenOrCreate, FileAccess.Write);
//  await response.Content.CopyToAsync(fileStream);
//  fileStream.Close();

//但考虑到下载的文件会过大，比如：3GB，这个时候首先HttpClient的缓冲区就不够用了，因为它最大设置的是：int.MaxValue=2^31-1≈2GB，看下面的报错代码：

//var httpClient = new HttpClient();
////默认缓冲大小为: 2147483647=int.MaxValue=2^31-1≈2GB，如果下载的文件过大就会报异常:  "Cannot write more bytes to the buffer than the configured maximum buffer size: 2147483647."
////可以手动设置缓冲区大小，但最大就是int.MaxValue，再大就会报错: "Buffering more than 2147483647 bytes is not supported."
//httpClient.MaxResponseContentBufferSize = 2L << 32; //调成4GB，发现报错

//这个时候我们就不能一次性获取Http响应报文的全部内容了，需要如下操作：

//var httpClient = new HttpClient();
//var url = "http://localhost:9000/bigdata.mp4";//3GB大小
////注意：因为太大，必须指定 HttpCompletionOption.ResponseHeadersRead，即：拿到响应头就返回
//var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
//var fileStream = new FileStream("e:\\bigdata.db", FileMode.OpenOrCreate, FileAccess.Write);
//// 虽然上面指定拿到响应头就返回，但这里依然可以拿到下载的文件流
//await response.Content.CopyToAsync(fileStream);
//fileStream.Close();

//现在，需要对这个大文件加上下载进度提示，我们需要事先获取文件的大小，这通过http响应头的Content-Length可以获取到（但并不总能获取到）：

//var httpClient = new HttpClient();
//var url = "http://localhost:9000/bigdata.mp4";
//var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
//var totalLength = response.Content.Headers.ContentLength;
//var contentStream = await response.Content.ReadAsStreamAsync();
//var fileStream = new FileStream("e:\\bigdata.db", FileMode.OpenOrCreate, FileAccess.Write);
//byte[] buffer = new byte[5 * 1024];//5KB缓存
//long readLength = 0L;
//int length;
//while ((length = await contentStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
//{
//    readLength += length;
//    if (totalLength > 0)
//    {
//        Console.WriteLine("下载进度: " + Math.Round((double)readLength / totalLength.Value * 100, 2) + "%");
//    }
//    else
//    {
//        Console.WriteLine("已下载: " + Math.Round((readLength / 1024.0), 2) + "KB");
//    }
//    fileStream.Write(buffer, 0, length);
//}
//fileStream.Close();

//6.3 通过Post请求数据： application/x-www-form-urlencoded
//var httpClient = new HttpClient();
//var url = "http://192.168.0.9:9000/Demo/PostUrlCode";
//var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
//{
//    new KeyValuePair<string, string>("name","小明"),
//    new KeyValuePair<string, string>("age","20")
//}));
//var str = await response.Content.ReadAsStringAsync();

//上面的请求报文:

//POST /Demo/PostUrlCode HTTP/1.1
//Host: 192.168.0.9:9000
//Content-Type: application/x-www-form-urlencoded
//Content-Length: 30

//name=%E5%B0%8F%E6%98%8E&age=20

//对应的asp.net core后台：

////注意：asp.net core webapi和mvc模式解析参数的时候存在差别，这里是asp.net core webapi
//[HttpPost]
//public string PostUrlCode([FromForm] string name, [FromForm] int age)
//{
//    var str = "";
//    foreach (var header in Request.Headers)
//    {
//        str += $"{header.Key}: {header.Value.ToString()}\r\n";
//    }
//    return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {name} {age} \r\n{str}";
//}

//http响应报文体：

//2021-08-19 19:35:04.804 小明 20 
//Content-Type: application/x-www-form-urlencoded
//Host: 192.168.0.9:9000
//Content-Length: 30

//如果想传递数组，发送请求时如下：

//var httpClient = new HttpClient();
//var url = "http://192.168.0.9:9000/Demo/PostUrlCode";
//var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
//{
//    new KeyValuePair<string, string>("names[0]","小明"),
//    new KeyValuePair<string, string>("names[1]","小红"),
//    new KeyValuePair<string, string>("age","20")
//}));
//var str = await response.Content.ReadAsStringAsync();

//对应的请求报文：

//POST /Demo/PostUrlCodeArr HTTP/1.1
//Host: 192.168.0.9:9000
//Content-Type: application/x-www-form-urlencoded
//Content-Length: 70

//names%5B0%5D=%E5%B0%8F%E6%98%8E&names%5B1%5D=%E5%B0%8F%E7%BA%A2&age=20

//此时asp.net core后台：

////注意：asp.net core webapi和mvc模式解析参数的时候存在差别，这里是asp.net core webapi
//[HttpPost]
//public string PostUrlCodeArr([FromForm] string[] names, [FromForm] int age)
//{
//    var str = "";
//    foreach (var header in Request.Headers)
//    {
//        str += $"{header.Key}: {header.Value.ToString()}\r\n";
//    }
//    return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {string.Join(",", names)} {age} \r\n{str}";
//}

//http响应报文体：

//2021-08-19 19:37:16.874 小明,小红 20 
//Content-Type: application/x-www-form-urlencoded
//Host: 192.168.0.9:9000
//Content-Length: 70

//6.4 使用Post请求数据：application/json
//var httpClient = new HttpClient();
//var url = "http://192.168.0.9:9000/Demo/PostUrlJson"
//var response = await httpClient.PostAsync(
//    url,
//    new StringContent(
//        Newtonsoft.Json.JsonConvert.SerializeObject(new { Name = "小明", Id = 1 }),
//        Encoding.UTF8,
//        "application/json")
//    );
//var str = await response.Content.ReadAsStringAsync();

//产生的请求报文：

//POST /Demo/PostUrlJson HTTP/1.1
//Host: 192.168.0.9:9000
//Content-Type: application/json; charset=utf-8
//Content-Length: 24

//{"Name":"小明","Id":1}

//此时asp.net core后台：

//[HttpPost]
//public string PostUrlJson([FromBody] RequestModel req)
//{
//    var str = "";
//    foreach (var header in Request.Headers)
//    {
//        str += $"{header.Key}: {header.Value.ToString()}\r\n";
//    }
//    return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {req.Name} {req.Id} \r\n{str}";
//}

//public class RequestModel
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//}

//返回的http响应体：

//2021-08-19 19:51:00.901 小明 1 
//Content-Type: application/json; charset=utf-8
//Host: 192.168.0.9:9000
//Content-Length: 24

//6.5 通过Post上传文件：multipart/form-data
//此处假设上传的文件不大。
//上传代码如下：

//var httpClient = new HttpClient();
//var url = "http://localhost:9000/Demo/PostMulti";
//var content = new MultipartFormDataContent();
//content.Add(new StringContent("小明"), "name");
//content.Add(new StringContent("18"), "age");
////注意：要指定filename，即：test.txt，否则后台不认为是一个文件，而是普通的参数
//content.Add(new ByteArrayContent(System.IO.File.ReadAllBytes("e:\\test.txt")), "file", "test.txt");
//var response = await httpClient.PostAsync(url, content);
//var str = await response.Content.ReadAsStringAsync();

//这里用到的文件很小：

//// test.txt
//this is file.

//产生的请求报文如下：

//POST /Demo/PostMulti HTTP/1.1
//Host: 192.168.0.9:9000
//Content-Type: multipart/form-data; boundary="ecc429f8-1f51-43a6-af5b-0fc8a88da513"
//Content-Length: 451

//--ecc429f8-1f51-43a6-af5b-0fc8a88da513
//Content-Type: text/plain; charset=utf-8
//Content-Disposition: form-data; name=name

//小明
//--ecc429f8-1f51-43a6-af5b-0fc8a88da513
//Content-Type: text/plain; charset=utf-8
//Content-Disposition: form-data; name=age

//18
//--ecc429f8-1f51-43a6-af5b-0fc8a88da513
//Content-Disposition: form-data; name=file; filename=test.txt; filename*=utf-8''test.txt

//this is file.
//--ecc429f8-1f51-43a6-af5b-0fc8a88da513--

//后台aspet core代码如下：

//[HttpPost]
//public string PostMulti([FromForm] string name, [FromForm] int age)
//{
//    var str = "";
//    foreach (var header in Request.Headers)
//    {
//        str += $"{header.Key}: {header.Value.ToString()}\r\n";
//    }
//    if (Request.Form.Files.Count > 0)
//    {
//        var file = Request.Form.Files[0];
//        var fileName = file.FileName;
//        var fileLength = file.Length;
//        using var stream = file.OpenReadStream();
//        var bytearr = new byte[fileLength];
//        stream.ReadAsync(bytearr);
//        var fileContent = Encoding.UTF8.GetString(bytearr);
//        str += "\r\n" + fileContent;
//    }
//    return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {name} {age} \r\n{str}";
//}

//对应的http响应报文体：

//2021-08-19 22:40:25.626  
//Content-Type: multipart/form-data; boundary="ecc429f8-1f51-43a6-af5b-0fc8a88da513"
//Host: 192.168.0.9:9000
//Content-Length: 451

//this is file.

//现在，客户端要上传一个大文件，该如何操作？
//我们知道作为一个web服务器一般是不会允许上传太大文件的，所以这里首先要说一下服务器端的限制。
//假设asp.net core直接在Kestrel下面运行，那么它将有如下限制：

//kestrel限制请求体最大为：28M；
//formbody最大为：128M
//先看第一个限制：
//可引发异常 “Microsoft.AspNetCore.Server.Kestrel.Core.BadHttpRequestException: Request body too large”

//public static IHostBuilder CreateHostBuilder(string[] args) =>
//   Host.CreateDefaultBuilder(args)
//       .ConfigureWebHostDefaults(webBuilder =>
//       {
//           webBuilder.ConfigureKestrel(options =>
//           {
//               //options.Limits.MaxRequestBodySize = 30000000L;//默认约28M
//               //options.Limits.MaxRequestBodySize = 2 * 2L << 30;//指定最大2G
//               options.Limits.MaxRequestBodySize = null;//去掉限制
//           });
//           webBuilder.UseStartup<Startup>();
//       });

//第二个限制：
//可引发异常：“Failed to read the request form. Multipart body length limit 134217728 exceeded.”

//public void ConfigureServices(IServiceCollection services)
//{
//   services.Configure<FormOptions>(x =>
//   {
//       //x.MultipartBodyLengthLimit = 134217728;//默认128MB
//       x.MultipartBodyLengthLimit = 5 * 2L << 30;//这里手动设置为5GB,这么大的数值仅用于演示
//   });
//   services.AddControllers();
//}

//除了服务器端的限制，客户端也有限制，那就是因为上传的文件太大，导致等待响应时间超时，引发的异常为：System.Threading.Tasks.TaskCanceledException:“A task was canceled.”。
//解决办法：

//var httpClient = new HttpClient();
//httpClient.Timeout = Timeout.InfiniteTimeSpan;//仅用于演示，将时间改为无限长

//其实上传大文件的时候遇到的限制，无非就是时间和空间的。在做项目的时候注意其配置就可以了。

//下面演示上传大文件：

//var httpClient = new HttpClient();
//httpClient.Timeout = Timeout.InfiniteTimeSpan;
//var url = "http://192.168.0.9:9000/Demo/PostMulti2";
//var content = new MultipartFormDataContent();
//content.Add(new StringContent("小明"), "name");
//content.Add(new StringContent("18"), "age");
//var filepath = @"E:\BaiduNetdiskDownload\Docker实战.pdf";//97.6MB
//filepath = @"E:\BaiduNetdiskDownload\dnSpy_v6.14.zip";//151.6MB
//filepath = @"E:\BaiduNetdiskDownload\dotnetfx35.exe";//231MB
//filepath = @"E:\BaiduNetdiskDownload\Photoshop_13_LS3（cs6）安装包.7z";//1.12GB
//filepath = @"E:\BaiduNetdiskDownload\cn_windows_10_business_editions_version_2004_updated_june_2020_x64_dvd_49d8dbba.iso";//4.83GB
//using var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
//using var streamContent = new StreamContent(fileStream, 2048);
//content.Add(streamContent, "file", "bigdata.db");
//var response = await httpClient.PostAsync(url, content);
//var str = await response.Content.ReadAsStringAsync();

//放开限制后，上面的最大文件4.83G也是可以上传的。

//对了，后端的代码为：

//[HttpPost]
//public async Task<string> PostMulti2([FromForm] string name, [FromForm] int age)
//{
//    var str = "";
//    Console.WriteLine($"{DateTime.Now.ToString("F")} comming... ");
//    foreach (var header in Request.Headers)
//    {
//        str += $"{header.Key}: {header.Value.ToString()}\r\n";
//    }
//    if (Request.Form.Files.Count > 0)
//    {
//        var file = Request.Form.Files[0];
//        var fileName = file.FileName;
//        var fileLength = file.Length;
//        using var stream = file.OpenReadStream();
//        var filepath = "e:\\bigdataupload.db";
//        if (System.IO.File.Exists(filepath))
//        {
//            System.IO.File.Delete(filepath);
//        }
//        using var destStream = new FileStream("e:\\bigdataupload.db", FileMode.CreateNew, FileAccess.Write);
//        await stream.CopyToAsync(destStream);
//        str += $"\r\nfileName={fileName}\r\n{fileLength}=fileLength";
//    }
//    else
//    {
//        str += "no file";
//    }
//    return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {name} {age} \r\n{str}";
//}

//七、使用DelegatingHandler实现http请求拦截管道
//类比asp.net core的请求管道，借助DelegatingHandler我们也能在HttpClient中轻松实现中间拦截。

//拦截的核心是使用DelegatingHandler，它继承自HttpMessageHandler，并且封装了一个HttpMessageHandler，这样就允许我们对HttpMessageHandler进行层层封装，每封装的一层就可认为是asp.net core中的中间件，
//封装完成后将最外层的DelegatingHandler交给HttpClient去使用便完成了构建过程。

//下面演示构造的两个中间件，拦截示意图：

//代码如下：

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        var httpClient = new HttpClient(new InterceptAMessageHandler(new InterceptBMessageHandler(new SocketsHttpHandler())));
//        var resposne = await httpClient.GetAsync("http://www.baidu.com");
//        Console.WriteLine("ok");
//    }
//}

//public class InterceptAMessageHandler : DelegatingHandler
//{
//    public InterceptAMessageHandler(HttpMessageHandler innerHandler) : base(innerHandler)
//    {
//    }

//    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//    {
//        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} InterceptAMessageHandler before Send");
//        var response = await base.SendAsync(request, cancellationToken);
//        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} InterceptAMessageHandler after Send");
//        return response;
//    }
//}

//public class InterceptBMessageHandler : DelegatingHandler
//{
//    public InterceptBMessageHandler(HttpMessageHandler innerHandler) : base(innerHandler)
//    {
//    }

//    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//    {
//        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} InterceptBMessageHandler before Send");
//        var response = await base.SendAsync(request, cancellationToken);
//        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} InterceptBMessageHandler after Send");
//        return response;
//    }
//}

//运行效果：


//八、FAQ
//8.1 向请求头里放非标准的 Authorization 引发报错
//参照：https://www.e-learn.cn/topic/3951198

//比如：

//var req = new HttpRequestMessage
//{
//    Method = HttpMethod.Get,
//    RequestUri = new Uri(url),
//};
//req.Headers.Add("Authorization", "test:123");
//// System.FormatException:“The format of value 'test:123' is invalid.”

//解决办法：使用 req.Headers.TryAddWithoutValidation("Authorization","test:123")
