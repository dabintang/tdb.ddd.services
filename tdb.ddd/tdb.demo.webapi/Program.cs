using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using SqlSugar.IOC;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.webapi;
using tdb.ddd.webapi.Common;
using tdb.demo.webapi;
using tdb.demo.webapi.Configs;

var builder = WebApplication.CreateBuilder(args);

//添加服务及使用
builder.RunWebApp(option =>
{
    //nlog
    option.LogOption.EnmLog = TdbWebAppBuilderOption.TdbEnmLog.NLog;
    //autofac容器
    option.IOCOption.EnmIOC = TdbWebAppBuilderOption.TdbEnmIOC.Autofac;
    //初始化配置（注：读取日志时，仅可使用日志和IOC）
    option.InitConfigAction = DemoConfig.Init;
    //hashid配置
    option.HashIDOption.Salt = "tangdabinok";
    //缓存
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Memory;
    //跨域请求
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //认证授
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => DemoConfig.App.Token.SecretKey);
    option.AuthOption.WhiteListIP = new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" };
    //接口入参验证
    option.IsUseParamValidate = true;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "演示服务";
        o.LstXmlCommentsFileName.Add("tdb.demo.webapi.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});

/******************************* 添加服务 start *************************************/

// Add services to the container.

////autofac容器注册
//builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
//{
//    //autofac注册模块
//    builder.RegisterModule<TdbAutofacModule>();
//});

////日志
//builder.Services.AddTdbNLogger();

////初始化配置
//DemoConfig.Init();

////hashid初始化
//TdbHashID.Init("tangdabinok");

////缓存服务
//builder.Services.AddTdbMemoryCache();

////参数验证
//builder.Services.AddTdbParamValidate();

////认证
//builder.Services.AddTdbAuthJwtBearer(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(DemoConfig.App.Token.SecretKey)));

////授权
//builder.Services.AddAuthorization(option =>
//{
//    //要求接口调用方IP为白名单IP
//    option.AddTdbAuthWhiteListIP(new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" });
//});

////设置允许所有来源跨域
//builder.Services.AddTdbCorsAllAllow();

//添加api版本控制及浏览服务
builder.Services.AddTdbApiVersionExplorer();

////swagger
//builder.Services.AddTdbSwaggerGenApiVer(o =>
//{
//    o.ServiceName = "演示服务";
//    o.LstXmlCommentsFileName.Add("tdb.demo.webapi.xml");
//});

//总线
builder.Services.AddTdbBusMediatR();

//添加SqlSugar服务（IOC模式）
builder.Services.AddTdbSqlSugar(c =>
{
    c.ConnectionString = DemoConfig.App.DBConnStr; //数据库连接字符串
    c.DbType = IocDbType.MySql;
    c.IsAutoCloseConnection = true;    //开启自动释放模式
});

// 注入请求上下文
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//httpclient
builder.Services.AddTdbHttpClient();

//压缩配置
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    //针对指定的MimeType来使用压缩策略
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
    options.EnableForHttps = true;
});

//针对不同的压缩类型，设置对应的压缩级别
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    //使用最优的方式进行压缩，即使花费的时间较长
    options.Level = CompressionLevel.Optimal;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    //使用最优的方式进行压缩，即使花费的时间较长
    options.Level = CompressionLevel.Optimal;
});

builder.Services.AddControllers(option =>
{
    //通用异常处理
    option.AddTdbGlobalExceptionFilter();
})
.AddJsonOptions(options =>
{
    //以下序列化选项使用通用库中一样的设置
    options.JsonSerializerOptions.PropertyNamingPolicy = CvtHelper.DefaultOptions.PropertyNamingPolicy;
    options.JsonSerializerOptions.IncludeFields = CvtHelper.DefaultOptions.IncludeFields;
    options.JsonSerializerOptions.Encoder = CvtHelper.DefaultOptions.Encoder;

    ////json字段名原样输出（null：不改变大小写；JsonNamingPolicy.CamelCase：驼峰法）
    //options.JsonSerializerOptions.PropertyNamingPolicy = null;
    ////包含变量
    //options.JsonSerializerOptions.IncludeFields = true;
    ////字符编码
    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});

/******************************* 添加服务 end *************************************/

var app = builder.Build();
TdbIOC.Init(app.Services);

/******************************* 配置管道 start *************************************/

// Configure the HTTP request pipeline.

//设置允许所有来源跨域
app.UseTdbCorsAllAllow();

//认证
app.UseAuthentication();

//授权
app.UseAuthorization();

//使用接口压缩
app.UseResponseCompression();

//swagger
app.UseTdbSwaggerAndUIApiVer();

////使用SqlSugar服务（IOC模式）
//app.UseTdbSqlSugar();

app.MapControllers();

/******************************* 配置管道 end *************************************/

app.Run();
