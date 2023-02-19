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

//��ӷ���ʹ��
builder.RunWebApp(option =>
{
    //nlog
    option.LogOption.EnmLog = TdbWebAppBuilderOption.TdbEnmLog.NLog;
    //autofac����
    option.IOCOption.EnmIOC = TdbWebAppBuilderOption.TdbEnmIOC.Autofac;
    //��ʼ�����ã�ע����ȡ��־ʱ������ʹ����־��IOC��
    option.InitConfigAction = DemoConfig.Init;
    //hashid����
    option.HashIDOption.Salt = "tangdabinok";
    //����
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Memory;
    //��������
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //��֤��
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => DemoConfig.App.Token.SecretKey);
    option.AuthOption.WhiteListIP = new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" };
    //�ӿ������֤
    option.IsUseParamValidate = true;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "��ʾ����";
        o.LstXmlCommentsFileName.Add("tdb.demo.webapi.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});

/******************************* ��ӷ��� start *************************************/

// Add services to the container.

////autofac����ע��
//builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
//{
//    //autofacע��ģ��
//    builder.RegisterModule<TdbAutofacModule>();
//});

////��־
//builder.Services.AddTdbNLogger();

////��ʼ������
//DemoConfig.Init();

////hashid��ʼ��
//TdbHashID.Init("tangdabinok");

////�������
//builder.Services.AddTdbMemoryCache();

////������֤
//builder.Services.AddTdbParamValidate();

////��֤
//builder.Services.AddTdbAuthJwtBearer(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(DemoConfig.App.Token.SecretKey)));

////��Ȩ
//builder.Services.AddAuthorization(option =>
//{
//    //Ҫ��ӿڵ��÷�IPΪ������IP
//    option.AddTdbAuthWhiteListIP(new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" });
//});

////��������������Դ����
//builder.Services.AddTdbCorsAllAllow();

//���api�汾���Ƽ��������
builder.Services.AddTdbApiVersionExplorer();

////swagger
//builder.Services.AddTdbSwaggerGenApiVer(o =>
//{
//    o.ServiceName = "��ʾ����";
//    o.LstXmlCommentsFileName.Add("tdb.demo.webapi.xml");
//});

//����
builder.Services.AddTdbBusMediatR();

//���SqlSugar����IOCģʽ��
builder.Services.AddTdbSqlSugar(c =>
{
    c.ConnectionString = DemoConfig.App.DBConnStr; //���ݿ������ַ���
    c.DbType = IocDbType.MySql;
    c.IsAutoCloseConnection = true;    //�����Զ��ͷ�ģʽ
});

// ע������������
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//httpclient
builder.Services.AddTdbHttpClient();

//ѹ������
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    //���ָ����MimeType��ʹ��ѹ������
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
    options.EnableForHttps = true;
});

//��Բ�ͬ��ѹ�����ͣ����ö�Ӧ��ѹ������
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    //ʹ�����ŵķ�ʽ����ѹ������ʹ���ѵ�ʱ��ϳ�
    options.Level = CompressionLevel.Optimal;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    //ʹ�����ŵķ�ʽ����ѹ������ʹ���ѵ�ʱ��ϳ�
    options.Level = CompressionLevel.Optimal;
});

builder.Services.AddControllers(option =>
{
    //ͨ���쳣����
    option.AddTdbGlobalExceptionFilter();
})
.AddJsonOptions(options =>
{
    //�������л�ѡ��ʹ��ͨ�ÿ���һ��������
    options.JsonSerializerOptions.PropertyNamingPolicy = CvtHelper.DefaultOptions.PropertyNamingPolicy;
    options.JsonSerializerOptions.IncludeFields = CvtHelper.DefaultOptions.IncludeFields;
    options.JsonSerializerOptions.Encoder = CvtHelper.DefaultOptions.Encoder;

    ////json�ֶ���ԭ�������null�����ı��Сд��JsonNamingPolicy.CamelCase���շ巨��
    //options.JsonSerializerOptions.PropertyNamingPolicy = null;
    ////��������
    //options.JsonSerializerOptions.IncludeFields = true;
    ////�ַ�����
    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});

/******************************* ��ӷ��� end *************************************/

var app = builder.Build();
TdbIOC.Init(app.Services);

/******************************* ���ùܵ� start *************************************/

// Configure the HTTP request pipeline.

//��������������Դ����
app.UseTdbCorsAllAllow();

//��֤
app.UseAuthentication();

//��Ȩ
app.UseAuthorization();

//ʹ�ýӿ�ѹ��
app.UseResponseCompression();

//swagger
app.UseTdbSwaggerAndUIApiVer();

////ʹ��SqlSugar����IOCģʽ��
//app.UseTdbSqlSugar();

app.MapControllers();

/******************************* ���ùܵ� end *************************************/

app.Run();
