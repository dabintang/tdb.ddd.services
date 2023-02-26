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
    option.ConfigureHashIDAction = (o) => o.Salt = AdminConfig.App.HashID.Salt;
    //����
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Memory;
    //����-MediatR
    option.BusOption.IsUseMediatR = true;
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //��������
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //��֤��Ȩ
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => AdminConfig.App.Token.SecretKey);
    option.AuthOption.WhiteListIP = new List<string>() { "127.0.0.1", "localhost", "::ffff:127.0.0.1" };
    //�ӿ������֤
    option.IsUseParamValidate = true;
    //ѹ��
    option.CompressionOption.ConfigureCompressionOptions = option.CompressionOption.DefaultConfigureCompressionOptions;
    option.CompressionOption.ConfigureProvider = option.CompressionOption.DefaultConfigureProvider;
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
