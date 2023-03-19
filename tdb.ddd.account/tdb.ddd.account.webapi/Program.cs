using Microsoft.Extensions.Options;
using SqlSugar.IOC;
using tdb.ddd.account.infrastructure.Config;
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
    option.InitConfigAction = AccountConfig.Init;
    //hashid����
    option.SetupHashID = (o) => o.Salt = AccountConfig.Common.HashID.Salt;
    //����
    option.CacheOption.EnmCache = TdbWebAppBuilderOption.TdbEnmCache.Redis;
    option.CacheOption.SetupRedis = (o) => o.ConnectionStrings = AccountConfig.Distributed.Redis.ConnStr;
    //����-MediatR
    option.BusOption.MediatROption = new TdbWebAppBuilderOption.TdbMediatROption();
    //����-DotNetCore.CAP
    option.BusOption.SetupDotNetCoreCAP = (o) =>
    {
        o.UseRedis(AccountConfig.Common.CAP.RedisConnStr);
        o.UseMySql(AccountConfig.Common.CAP.DBConnStr);
        option.BusOption.DefaultCapOptionsWithoutTransportAndStorage(o);
        o.DefaultGroupName = "account";
    };
    //SqlSugar��IOCģʽ��
    option.SetupSqlSugar = () => builder.Services.AddTdbSqlSugar(c =>
    {
        c.ConnectionString = AccountConfig.Distributed.DB.ConnStr; //���ݿ������ַ���
        c.DbType = IocDbType.MySql;
        c.IsAutoCloseConnection = true;    //�����Զ��ͷ�ģʽ
    });
    //��������
    option.CorsOption.SetupCors = option.CorsOption.SetupCorsAllowAll;
    option.CorsOption.UseCors = option.CorsOption.UseCorsAllAllow;
    //��֤��Ȩ
    option.AuthOption.SetupJwtBearer = (jwtBearerOption) => option.AuthOption.DefaultJwtBearerOptions(jwtBearerOption, () => AccountConfig.Common.Token.SecretKey);
    option.AuthOption.GetWhiteListIP = () => AccountConfig.Common.WhiteListIP;
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
        o.LstXmlCommentsFileName.Add("tdb.ddd.account.domain.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.account.application.contracts.xml");
        o.LstXmlCommentsFileName.Add("tdb.ddd.account.webapi.xml");
    };
    option.SwaggerOption.RoutePrefix = "tdbswagger";
});
