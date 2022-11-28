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

            /******************************* 添加服务 start *************************************/

            // Add services to the container.

            //autofac容器注册
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                //autofac注册模块
                builder.RegisterModule<TdbAutofacModule>();
            });

            //日志
            builder.Services.AddTdbNLogger();

            //配置服务
            builder.Services.AddTdbAppsettingsConfig();
            //初始化配置
            ToolsConfig.Init();

            //缓存
            builder.Services.AddTdbMemoryCache();

            //总线
            builder.Services.AddTdbBusMediatR();

            //添加SqlSugar服务（IOC模式）
            builder.Services.AddTdbSqlSugar(c =>
            {
                c.ConnectionString = ToolsConfig.App.DBConnStr; //数据库连接字符串
                c.DbType = IocDbType.MySql;
                c.IsAutoCloseConnection = true;    //开启自动释放模式
            });

            // 注入请求上下文
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            /******************************* 添加服务 end *************************************/

            var app = builder.Build();
            TdbIOC.Init(app.Services);

            /******************************* 配置管道 start *************************************/

            // Configure the HTTP request pipeline.

            /******************************* 配置管道 end *************************************/

            Console.WriteLine("请输入指令");
            Console.WriteLine("1：初始化数据");

            //获取输入的指令
            var command = Console.ReadLine();
            switch (command)
            {
                case "1": //1：初始化数据
                    new ToolsApp().InitDataAsync().Wait();
                    break;
                default:
                    Console.WriteLine("退出");
                    break;
            }

            //app.Run();
        }
    }
}