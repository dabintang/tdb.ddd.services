using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using tdb.common;
using tdb.ddd.admin.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.webapi;

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

//��ʼ������
AdminConfig.Init();

//��ʼ��hash id
TdbHashID.Init(AdminConfig.App.HashID.Salt);

//�������
builder.Services.AddTdbMemoryCache();
//builder.Services.AddTdbRedisCache(AccountConfig.App.RedisConnStr.ToArray());

//������֤
builder.Services.AddTdbParamValidate();

//��Ȩ
builder.Services.AddTdbAuthJwtBearer(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AdminConfig.App.Token.SecretKey)));

//��Ȩ
builder.Services.AddAuthorization();

//��������������Դ����
builder.Services.AddTdbCorsAllAllow();

//���api�汾���Ƽ��������
builder.Services.AddTdbApiVersionExplorer();

//swagger
builder.Services.AddTdbSwaggerGenApiVer(o =>
{
    o.ServiceName = "�˻�΢����";
    o.LstXmlCommentsFileName.Add("tdb.ddd.admin.domain.contracts.xml");
    o.LstXmlCommentsFileName.Add("tdb.ddd.admin.application.contracts.xml");
    o.LstXmlCommentsFileName.Add("tdb.ddd.admin.webapi.xml");
});

//����
builder.Services.AddTdbBusMediatR();

////���SqlSugar����IOCģʽ��
//builder.Services.AddTdbSqlSugar(c =>
//{
//    c.ConnectionString = AccountConfig.App.DB.ConnStr; //���ݿ������ַ���
//    c.DbType = IocDbType.MySql;
//    c.IsAutoCloseConnection = true;    //�����Զ��ͷ�ģʽ
//});

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

app.MapControllers();

/******************************* ���ùܵ� end *************************************/

app.Run();
