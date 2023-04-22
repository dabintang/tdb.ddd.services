using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
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

        ///// <summary>
        ///// 排序
        ///// </summary>
        ///// <typeparam name="TSource">需排序的对象类型</typeparam>
        ///// <typeparam name="TKey">排序字段类型</typeparam>
        ///// <param name="list">列表</param>
        ///// <param name="sortCode">排序方式（1：升序；2：降序）</param>
        ///// <param name="keySelector">排序字段选择器</param>
        ///// <returns></returns>
        //public static IOrderedEnumerable<TSource> Sort<TSource, TKey>(this IEnumerable<TSource> list, EnmTdbSort sortCode, Func<TSource, TKey> keySelector)
        //{
        //    switch (sortCode)
        //    {
        //        case EnmTdbSort.Asc:
        //            if (list is IOrderedEnumerable<TSource> listAsc)
        //            {
        //                return listAsc.ThenBy(m => keySelector(m));
        //            }
        //            else
        //            {
        //                return list.OrderBy(m => keySelector(m));
        //            }
        //        case EnmTdbSort.Desc:
        //            if (list is IOrderedEnumerable<TSource> listDesc)
        //            {
        //                return listDesc.ThenByDescending(m => keySelector(m));
        //            }
        //            else
        //            {
        //                return list.OrderByDescending(m => keySelector(m));
        //            }
        //        default:
        //            throw new TdbException($"不支持的排序方式{sortCode}");
        //    }
        //}
    }
}
