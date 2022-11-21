using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 总线服务扩展
    /// </summary>
    public static class TdbBusExtensions
    {
        /// <summary>
        /// 添加MediatR服务
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="assemblies">含实现了总线相关接口的类的程序集</param>
        public static void AddTdbBusMediatR(this IServiceCollection services, params Assembly[] assemblies)
        {
            //添加MediatR服务
            services.AddMediatR(assemblies);
        }
    }
}
