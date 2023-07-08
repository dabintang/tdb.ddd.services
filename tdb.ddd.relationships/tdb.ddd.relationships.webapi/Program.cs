using SqlSugar.IOC;
using tdb.ddd.relationships.infrastructure.Config;
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
    option.InitConfigAction = RelationshipsConfig.Init;
    //hashid配置
    option.SetupHashID = (o) => o.Salt = RelationshipsConfig.Common.HashID.Salt;
    //缓存
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Redis;
    option.CacheOption.SetupRedis = (o) => o.ConnectionStrings = RelationshipsConfig.Distributed.Redis.ConnStr;
    //总线-MediatR
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //总线-DotNetCore.CAP
    option.BusOption.CAPOption = new TdbWebAppBuilderOption.TdbDotNetCoreCAPOption((o) =>
    {
        o.UseRedis(RelationshipsConfig.Common.CAP.RedisConnStr);
        o.UseMySql(RelationshipsConfig.Common.CAP.DBConnStr);
        o.DefaultGroupName = "account";
    });
    //SqlSugar（IOC模式）
    option.SetupSqlSugar = () => builder.Services.AddTdbSqlSugar(c =>
    {
        c.ConnectionString = RelationshipsConfig.Distributed.DB.ConnStr; //数据库连接字符串
        c.DbType = IocDbType.MySql;
        c.IsAutoCloseConnection = true;    //开启自动释放模式
    });
    //跨域请求
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //认证授权
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => RelationshipsConfig.Common.Token.SecretKey);
    //option.AuthOption.GetWhiteListIP = () => RelationshipsConfig.Common.WhiteListIP;
    //接口入参验证
    option.IsUseParamValidate = true;
    //压缩
    option.CompressionOption.SetupCompression = option.CompressionOption.DefaultSetupCompression;
    option.CompressionOption.SetupProvider = option.CompressionOption.DefaultSetupProvider;
    //响应缓存
    //option.IsUseResponseCaching = true;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "账户服务";
        o.LstXmlCommentsFileName.Add("tdb.ddd.relationships.domain.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.relationships.application.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.relationships.webapi.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});
