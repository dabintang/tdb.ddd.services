using SqlSugar.IOC;
using tdb.ddd.files.infrastructure.Config;
using tdb.ddd.repository.sqlsugar;
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
    option.InitConfigAction = FilesConfig.Init;
    //hashid����
    option.SetupHashID = (o) => o.Salt = FilesConfig.Common.HashID.Salt;
    //����
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Redis;
    option.CacheOption.SetupRedis = (o) => o.ConnectionStrings = FilesConfig.Distributed.Redis.ConnStr;
    //����-MediatR
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //SqlSugar��IOCģʽ��
    option.SetupSqlSugar = () => builder.Services.AddTdbSqlSugar(c =>
    {
        c.ConnectionString = FilesConfig.Distributed.DB.ConnStr; //���ݿ������ַ���
        c.DbType = IocDbType.MySql;
        c.IsAutoCloseConnection = true;    //�����Զ��ͷ�ģʽ
    });
    //����-DotNetCore.CAP
    option.BusOption.SetupDotNetCoreCAP = (o) =>
    {
        o.UseRedis(FilesConfig.Common.CAP.RedisConnStr);
        o.UseMySql(FilesConfig.Common.CAP.DBConnStr);
    };
    //��������
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //��֤��Ȩ
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => FilesConfig.Common.Token.SecretKey);
    //�ӿ������֤
    option.IsUseParamValidate = true;
    //ѹ��
    option.CompressionOption.SetupCompression = option.CompressionOption.DefaultSetupCompression;
    option.CompressionOption.SetupProvider = option.CompressionOption.DefaultSetupProvider;
    //��Ӧ����
    option.IsUseResponseCaching = true;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "�ļ�����";
        o.LstXmlCommentsFileName.Add("tdb.ddd.files.domain.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.files.application.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.files.webapi.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});
