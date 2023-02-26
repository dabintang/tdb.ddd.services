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
    option.ConfigureHashIDAction = (o) => o.Salt = AdminConfig.App.HashID.Salt;
    //缓存
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Memory;
    //总线-MediatR
    option.BusOption.IsUseMediatR = true;
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //跨域请求
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //认证授权
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => AdminConfig.App.Token.SecretKey);
    option.AuthOption.WhiteListIP = new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" };
    //接口入参验证
    option.IsUseParamValidate = true;
    //压缩
    option.CompressionOption.ConfigureCompressionOptions = option.CompressionOption.DefaultConfigureCompressionOptions;
    option.CompressionOption.ConfigureProvider = option.CompressionOption.DefaultConfigureProvider;
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
