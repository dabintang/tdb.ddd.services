
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using tdb.common;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.webapi;
using tdb.ddd.files.infrastructure.Config;
using SqlSugar.IOC;

var builder = WebApplication.CreateBuilder(args);

/******************************* 添加服务 start *************************************/

// Add services to the container.

//autofac容器注册
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    //autofac注册模块
    builder.RegisterModule<TdbAutofacModule>();
});

//日志
builder.Services.AddTdbNLogger();

//配置服务
builder.Services.AddTdbAppsettingsConfig();
//初始化配置
FilesConfig.Init();

//缓存服务
builder.Services.AddTdbMemoryCache();
//builder.Services.AddTdbRedisCache(AccountConfig.App.RedisConnStr.ToArray());

//参数验证
builder.Services.AddTdbParamValidate();

//验权
builder.Services.AddTdbAuthJwtBearer(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(FilesConfig.App.Token.SecretKey)));

//授权
builder.Services.AddAuthorization(options =>
{
    //客户端IP要求与token中一致
    options.AddTdbAuthClientIP();
});

//设置允许所有来源跨域
builder.Services.AddTdbCorsAllAllow();

//添加api版本控制及浏览服务
builder.Services.AddTdbApiVersionExplorer();

//swagger
builder.Services.AddTdbSwaggerGenApiVer(o =>
{
    o.ServiceName = "账户微服务";
    o.LstXmlCommentsFileName.Add("tdb.ddd.files.domain.contracts.xml");
    o.LstXmlCommentsFileName.Add("tdb.ddd.files.application.contracts.xml");
    o.LstXmlCommentsFileName.Add("tdb.ddd.files.webapi.xml");
});

//总线
builder.Services.AddTdbBusMediatR();

//添加SqlSugar服务（IOC模式）
builder.Services.AddTdbSqlSugar(c =>
{
    c.ConnectionString = FilesConfig.App.DB.ConnStr; //数据库连接字符串
    c.DbType = IocDbType.MySql;
    c.IsAutoCloseConnection = true;    //开启自动释放模式
});

//响应缓存
builder.Services.AddResponseCaching();

// 注入请求上下文
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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

//响应缓存
app.UseResponseCaching();

//使用接口压缩
app.UseResponseCompression();

//swagger
app.UseTdbSwaggerAndUIApiVer();

app.MapControllers();

/******************************* 配置管道 end *************************************/

app.Run();
