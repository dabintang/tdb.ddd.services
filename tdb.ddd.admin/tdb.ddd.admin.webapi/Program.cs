using tdb.ddd.admin.infrastructure.Config;
using tdb.ddd.webapi.Common;

var builder = WebApplication.CreateBuilder(args);


//��ӷ���ʹ��
builder.RunWebApp(option =>
{
    //nlog
    option.LogOption.EnmLog = TdbWebAppBuilderOption.TdbEnmLog.NLog;
    //autofac����
    option.IOCOption.EnmIOC = TdbWebAppBuilderOption.TdbEnmIOC.Autofac;
    //��ʼ�����ã�ע����ʼ������ʱ������ʹ����־��IOC��
    option.InitConfigAction = AdminConfig.Init;
    //hashid����
    option.SetupHashID = (o) => o.Salt = AdminConfig.Common?.HashID?.Salt ?? AdminConfig.App.HashID.Salt;
    //����
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Memory;
    //����-MediatR
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //����-DotNetCore.CAP
    option.BusOption.CAPOption = new TdbWebAppBuilderOption.TdbDotNetCoreCAPOption((o) =>
    {
        o.UseRedis(AdminConfig.Common?.CAP?.RedisConnStr ?? AdminConfig.App.CAP.RedisConnStr);
        o.UseMySql(AdminConfig.Common?.CAP?.DBConnStr ?? AdminConfig.App.CAP.DBConnStr);
        o.DefaultGroupName = "admin";
    });
    //��������
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //��֤��Ȩ
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => AdminConfig.Common?.Token?.SecretKey ?? AdminConfig.App.Token.SecretKey);
    //������IP
    option.AuthOption.GetWhiteListIP = () => AdminConfig.App.WhiteListIP;
    //�ӿ������֤
    option.IsUseParamValidate = true;
    //ѹ��
    option.CompressionOption.SetupCompression = option.CompressionOption.DefaultSetupCompression;
    option.CompressionOption.SetupProvider = option.CompressionOption.DefaultSetupProvider;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "��ά����";
        o.LstXmlCommentsFileName.Add("tdb.ddd.admin.domain.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.admin.application.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.admin.webapi.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});
