using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure;

namespace tdb.ddd.repository.sqlsugar
{
    /// <summary>
    /// SqlSugar扩展类
    /// </summary>
    public static class TdbSqlSugarExtensions
    {
        /// <summary>
        /// 添加SqlSugar服务（IOC模式）
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="setIOCConfig">设置IOC配置</param>
        /// <param name="configSugar">配置SqlSugar（默认为按Trace级别打印执行的SQL）</param>
        public static void AddTdbSqlSugar(this IServiceCollection services, Action<IocConfig> setIOCConfig, Action<SqlSugarClient>? configSugar = null)
        {
            //设置IOC配置
            var iocConfig = new IocConfig();
            setIOCConfig(iocConfig);

            //SqlSugar.IOC
            services.AddSqlSugar(iocConfig);

            //配置SqlSugar
            services.ConfigurationSugar(db =>
            {
                if (configSugar == null)
                {
                    //默认打印AOP日志
                    if (TdbLogger.Ins.IsTraceEnabled)
                    {
                        db.Aop.OnLogExecuting = (sql, pars) =>
                        {
                            TdbLogger.Ins.Trace($"执行SQL：{sql}{Environment.NewLine}{db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value))}");
                        };
                    }
                }
                else
                {
                    configSugar(db);
                }
            });
        }

        ///// <summary>
        ///// 使用SqlSugar服务（IOC模式）
        ///// </summary>
        ///// <param name="app"></param>
        //public static void UseTdbSqlSugar(this IApplicationBuilder app)
        //{
        //    TdbDbScoped.Init(app.ApplicationServices);
        //}
    }
}
