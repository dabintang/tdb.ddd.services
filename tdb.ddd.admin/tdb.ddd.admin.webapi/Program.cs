using tdb.ddd.admin.infrastructure.Config;
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
    option.InitConfigAction = AdminConfig.Init;
    //hashid配置
    option.SetupHashID = (o) => o.Salt = AdminConfig.Common?.HashID?.Salt ?? AdminConfig.App.HashID.Salt;
    //缓存
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Memory;
    //总线-MediatR
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //总线-DotNetCore.CAP
    option.BusOption.CAPOption = new TdbWebAppBuilderOption.TdbDotNetCoreCAPOption((o) =>
    {
        o.UseRedis(AdminConfig.Common?.CAP?.RedisConnStr ?? AdminConfig.App.CAP.RedisConnStr);
        o.UseMySql(AdminConfig.Common?.CAP?.DBConnStr ?? AdminConfig.App.CAP.DBConnStr);
        o.DefaultGroupName = "admin";
    });
    //跨域请求
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //认证授权
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => AdminConfig.Common?.Token?.SecretKey ?? AdminConfig.App.Token.SecretKey);
    //白名单IP
    option.AuthOption.GetWhiteListIP = () => AdminConfig.App.WhiteListIP;
    //接口入参验证
    option.IsUseParamValidate = true;
    //压缩
    option.CompressionOption.SetupCompression = option.CompressionOption.DefaultSetupCompression;
    option.CompressionOption.SetupProvider = option.CompressionOption.DefaultSetupProvider;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "运维服务";
        o.LstXmlCommentsFileName.Add("tdb.ddd.admin.domain.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.admin.application.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.admin.webapi.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});
