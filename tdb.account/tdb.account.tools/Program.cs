using Autofac.Extensions.DependencyInjection;
using Autofac;
using tdb.account.application;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.repository.sqlsugar;
using tdb.account.tools.Configs;
using SqlSugar.IOC;
using System.Threading;
using System.Diagnostics;

namespace tdb.account.tools
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /******************************* ��ӷ��� start *************************************/

            // Add services to the container.

            //autofac����ע��
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                //autofacע��ģ��
                builder.RegisterModule<TdbAutofacModule>();
            });

            //��־
            builder.Services.AddTdbNLogger();

            //���÷���
            builder.Services.AddTdbAppsettingsConfig();
            //��ʼ������
            ToolsConfig.Init();

            //����
            builder.Services.AddTdbMemoryCache();

            //����
            builder.Services.AddTdbBusMediatR();

            //���SqlSugar����IOCģʽ��
            builder.Services.AddTdbSqlSugar(c =>
            {
                c.ConnectionString = ToolsConfig.App.DBConnStr; //���ݿ������ַ���
                c.DbType = IocDbType.MySql;
                c.IsAutoCloseConnection = true;    //�����Զ��ͷ�ģʽ
            });

            // ע������������
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            /******************************* ��ӷ��� end *************************************/

            var app = builder.Build();
            TdbIOC.Init(app.Services);

            /******************************* ���ùܵ� start *************************************/

            // Configure the HTTP request pipeline.

            /******************************* ���ùܵ� end *************************************/

            Console.WriteLine("������ָ��");
            Console.WriteLine("1����ʼ������");

            //��ȡ�����ָ��
            var command = Console.ReadLine();
            switch (command)
            {
                case "1": //1����ʼ������
                    new ToolsApp().InitDataAsync().Wait();
                    break;
                default:
                    Console.WriteLine("�˳�");
                    break;
            }

            //app.Run();
        }
    }
}