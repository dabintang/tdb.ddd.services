using SqlSugar.IOC;
using tdb.ddd.account.infrastructure.Config;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.webapi.Common;

var builder = WebApplication.CreateBuilder(args);

//添加服务及使用
builder.RunWebApp(option =>
{
    //nlog
    option.LogOption.EnmLog = TdbWebAppBuilderOption.TdbEnmLog.NLog;
    //autofac容器
    option.IOCOption.EnmIOC = TdbWebAppBuilderOption.TdbEnmIOC.Autofac;
    //初始化配置（注：初始化配置时，仅可使用日志和IOC）
    option.InitConfigAction = AccountConfig.Init;
    //hashid配置
    option.ConfigureHashIDAction = (o) => o.Salt = AccountConfig.Common.HashID.Salt;
    //缓存
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Redis;
    option.CacheOption.ConfigureRedisAction = (o) => o.ConnectionStrings = AccountConfig.Distributed.Redis.ConnStr;
    //总线-MediatR
    option.BusOption.IsUseMediatR = true;
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //SqlSugar（IOC模式）
    option.ConfigureSqlSugarAction = () => builder.Services.AddTdbSqlSugar(c =>
    {
        c.ConnectionString = AccountConfig.Distributed.DB.ConnStr; //数据库连接字符串
        c.DbType = IocDbType.MySql;
        c.IsAutoCloseConnection = true;    //开启自动释放模式
    });
    //跨域请求
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //认证授权
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => AccountConfig.Common.Token.SecretKey);
    option.AuthOption.WhiteListIP = new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" };
    //接口入参验证
    option.IsUseParamValidate = true;
    //压缩
    option.CompressionOption.ConfigureCompressionOptions = option.CompressionOption.DefaultConfigureCompressionOptions;
    option.CompressionOption.ConfigureProvider = option.CompressionOption.DefaultConfigureProvider;
    //响应缓存
    //option.IsUseResponseCaching = true;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "账户服务";
        o.LstXmlCommentsFileName.Add("tdb.ddd.account.domain.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.account.application.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.account.webapi.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});
