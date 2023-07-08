using SqlSugar.IOC;
using tdb.ddd.relationships.infrastructure.Config;
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
    option.InitConfigAction = RelationshipsConfig.Init;
    //hashid����
    option.SetupHashID = (o) => o.Salt = RelationshipsConfig.Common.HashID.Salt;
    //����
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Redis;
    option.CacheOption.SetupRedis = (o) => o.ConnectionStrings = RelationshipsConfig.Distributed.Redis.ConnStr;
    //����-MediatR
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //����-DotNetCore.CAP
    option.BusOption.CAPOption = new TdbWebAppBuilderOption.TdbDotNetCoreCAPOption((o) =>
    {
        o.UseRedis(RelationshipsConfig.Common.CAP.RedisConnStr);
        o.UseMySql(RelationshipsConfig.Common.CAP.DBConnStr);
        o.DefaultGroupName = "account";
    });
    //SqlSugar��IOCģʽ��
    option.SetupSqlSugar = () => builder.Services.AddTdbSqlSugar(c =>
    {
        c.ConnectionString = RelationshipsConfig.Distributed.DB.ConnStr; //���ݿ������ַ���
        c.DbType = IocDbType.MySql;
        c.IsAutoCloseConnection = true;    //�����Զ��ͷ�ģʽ
    });
    //��������
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //��֤��Ȩ
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => RelationshipsConfig.Common.Token.SecretKey);
    //option.AuthOption.GetWhiteListIP = () => RelationshipsConfig.Common.WhiteListIP;
    //�ӿ������֤
    option.IsUseParamValidate = true;
    //ѹ��
    option.CompressionOption.SetupCompression = option.CompressionOption.DefaultSetupCompression;
    option.CompressionOption.SetupProvider = option.CompressionOption.DefaultSetupProvider;
    //��Ӧ����
    //option.IsUseResponseCaching = true;
    //swagger
    option.SwaggerOption.EnmSwagger = TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer;
    option.SwaggerOption.SetupSwagger = (o) =>
    {
        o.ServiceName = "�˻�����";
        o.LstXmlCommentsFileName.Add("tdb.ddd.relationships.domain.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.relationships.application.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.relationships.webapi.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});
