using SqlSugar.IOC;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.webapi.Common;
using tdb.demo.webapi.Configs;

var builder = WebApplication.CreateBuilder(args);

//��ӷ���ʹ��
builder.RunWebApp(option =>
{
    //nlog
    option.LogOption.EnmLog = TdbWebAppBuilderOption.TdbEnmLog.NLog;
    //autofac����
    option.IOCOption.EnmIOC = TdbWebAppBuilderOption.TdbEnmIOC.Autofac;
    //��ʼ�����ã�ע����ʼ������ʱ������ʹ����־��IOC��
    option.InitConfigAction = DemoConfig.Init;
    //hashid����
    option.SetupHashID = (o) => o.Salt = "tangdabinok";
    //����
    //option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Memory;
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Redis;
    option.CacheOption.SetupRedis = (o) => o.ConnectionStrings = new List<string>() { "127.0.0.1,defaultDatabase=0,idleTimeout=30000,poolsize=10,prefix=BaseTest_" };
    //����-MediatR
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //����-DotNetCore.CAP
    option.BusOption.CAPOption = new TdbWebAppBuilderOption.TdbDotNetCoreCAPOption((o) =>
    {
        o.UseRedis("127.0.0.1,defaultDatabase=0,password=");
        o.UseMySql("server=127.0.0.1;database=tdb.ddd.cap;user id=root;password=abc123456;Pooling=True;Allow User Variables=True;");
    });
    //SqlSugar��IOCģʽ��
    option.SetupSqlSugar = () => builder.Services.AddTdbSqlSugar(c =>
    {
        c.ConnectionString = DemoConfig.App!.DBConnStr; //���ݿ������ַ���
        c.DbType = IocDbType.MySql;
        c.IsAutoCloseConnection = true;    //�����Զ��ͷ�ģʽ
    });
    //��������
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //��֤��
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => DemoConfig.App!.Token!.SecretKey);
    option.AuthOption.GetWhiteListIP = () => new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" };
    //�ӿ������֤
    option.IsUseParamValidate = true;
	//ѹ��
    option.CompressionOption.SetupCompression = option.CompressionOption.DefaultSetupCompression;
    option.CompressionOption.SetupProvider = option.CompressionOption.DefaultSetupProvider;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "��ʾ����";
        o.LstXmlCommentsFileName.Add("tdb.demo.webapi.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.contracts.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});
