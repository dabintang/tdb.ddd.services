
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

/******************************* ��ӷ��� start *************************************/

// Add services to the container.

//autofac����ע��
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    //autofacע��ģ��
    builder.RegisterModule<TdbAutofacModule>();
});

//��־
builder.Services.AddTdbNLogger();

//���÷���
builder.Services.AddTdbAppsettingsConfig();
//��ʼ������
FilesConfig.Init();

//�������
builder.Services.AddTdbMemoryCache();
//builder.Services.AddTdbRedisCache(AccountConfig.App.RedisConnStr.ToArray());

//������֤
builder.Services.AddTdbParamValidate();

//��Ȩ
builder.Services.AddTdbAuthJwtBearer(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(FilesConfig.App.Token.SecretKey)));

//��Ȩ
builder.Services.AddAuthorization(options =>
{
    //�ͻ���IPҪ����token��һ��
    options.AddTdbAuthClientIP();
});

//��������������Դ����
builder.Services.AddTdbCorsAllAllow();

//���api�汾���Ƽ��������
builder.Services.AddTdbApiVersionExplorer();

//swagger
builder.Services.AddTdbSwaggerGenApiVer(o =>
{
    o.ServiceName = "�˻�΢����";
    o.LstXmlCommentsFileName.Add("tdb.ddd.files.domain.contracts.xml");
    o.LstXmlCommentsFileName.Add("tdb.ddd.files.application.contracts.xml");
    o.LstXmlCommentsFileName.Add("tdb.ddd.files.webapi.xml");
});

//����
builder.Services.AddTdbBusMediatR();

//���SqlSugar����IOCģʽ��
builder.Services.AddTdbSqlSugar(c =>
{
    c.ConnectionString = FilesConfig.App.DB.ConnStr; //���ݿ������ַ���
    c.DbType = IocDbType.MySql;
    c.IsAutoCloseConnection = true;    //�����Զ��ͷ�ģʽ
});

//��Ӧ����
builder.Services.AddResponseCaching();

// ע������������
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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

//��Ӧ����
app.UseResponseCaching();

//ʹ�ýӿ�ѹ��
app.UseResponseCompression();

//swagger
app.UseTdbSwaggerAndUIApiVer();

app.MapControllers();

/******************************* ���ùܵ� end *************************************/

app.Run();
