using SqlSugar.IOC;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.webapi.Common;
using tdb.demo.webapi.Configs;

var builder = WebApplication.CreateBuilder(args);

//添加服务及使用
builder.RunWebApp(option =>
{
    //nlog
    option.LogOption.EnmLog = TdbWebAppBuilderOption.TdbEnmLog.NLog;
    //autofac容器
    option.IOCOption.EnmIOC = TdbWebAppBuilderOption.TdbEnmIOC.Autofac;
    //初始化配置（注：初始化配置时，仅可使用日志和IOC）
    option.InitConfigAction = DemoConfig.Init;
    //hashid配置
    option.SetupHashID = (o) => o.Salt = "tangdabinok";
    //缓存
    //option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Memory;
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Redis;
    option.CacheOption.SetupRedis = (o) => o.ConnectionStrings = new List<string>() { "127.0.0.1,defaultDatabase=0,idleTimeout=30000,poolsize=10,prefix=BaseTest_" };
    //总线-MediatR
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //总线-DotNetCore.CAP
    option.BusOption.CAPOption = new TdbWebAppBuilderOption.TdbDotNetCoreCAPOption((o) =>
    {
        o.UseRedis("127.0.0.1,defaultDatabase=0,password=");
        o.UseMySql("server=127.0.0.1;database=tdb.ddd.cap;user id=root;password=abc123456;Pooling=True;Allow User Variables=True;");
    });
    //SqlSugar（IOC模式）
    option.SetupSqlSugar = () => builder.Services.AddTdbSqlSugar(c =>
    {
        c.ConnectionString = DemoConfig.App!.DBConnStr; //数据库连接字符串
        c.DbType = IocDbType.MySql;
        c.IsAutoCloseConnection = true;    //开启自动释放模式
    });
    //跨域请求
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //认证授
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => DemoConfig.App!.Token!.SecretKey);
    option.AuthOption.GetWhiteListIP = () => new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" };
    //接口入参验证
    option.IsUseParamValidate = true;
	//压缩
    option.CompressionOption.SetupCompression = option.CompressionOption.DefaultSetupCompression;
    option.CompressionOption.SetupProvider = option.CompressionOption.DefaultSetupProvider;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "演示服务";
        o.LstXmlCommentsFileName.Add("tdb.demo.webapi.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.contracts.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});
