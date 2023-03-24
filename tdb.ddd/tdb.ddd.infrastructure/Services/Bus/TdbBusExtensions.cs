using DotNetCore.CAP;
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
        /// <param name="module">用到MediatR的程序集模块</param>
        public static void AddTdbBusMediatR(this IServiceCollection services, TdbBusAssemblyModule? module = null)
        {
            module ??= new TdbBusAssemblyModule();

            //添加MediatR服务
            AddTdbBusMediatR(services, module.GetRegisterAssemblys);
        }

        /// <summary>
        /// 添加MediatR服务
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="registerAssemblys">注册的程序集集合</param>
        public static void AddTdbBusMediatR(this IServiceCollection services, Func<List<Assembly>> registerAssemblys)
        {
            var assemblies = registerAssemblys();

            //添加MediatR服务
            services.AddMediatR(o =>
            {
                o.RegisterServicesFromAssemblies(assemblies.ToArray());
            });
        }

        /// <summary>
        /// 添加DotNetCore.CAP服务
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="setupDotNetCoreCAP">CAP设置方法</param>
        /// <param name="module">用到MediatR的程序集模块</param>
        public static CapBuilder AddTdbBusCAP(this IServiceCollection services, Action<CapOptions> setupDotNetCoreCAP, TdbBusAssemblyModule? module = null)
        {
            module ??= new TdbBusAssemblyModule();

            //添加MediatR服务
            return AddTdbBusCAP(services, setupDotNetCoreCAP, module.GetRegisterAssemblys);
        }

        /// <summary>
        /// 添加CAP服务
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="setupDotNetCoreCAP">CAP设置方法</param>
        /// <param name="registerAssemblys">注册的程序集集合</param>
        public static CapBuilder AddTdbBusCAP(this IServiceCollection services, Action<CapOptions> setupDotNetCoreCAP, Func<List<Assembly>> registerAssemblys)
        {
            var assemblies = registerAssemblys();

            //添加DotNetCore.CAP服务
            var capBuilder = services.AddCap(setupDotNetCoreCAP).AddSubscribeFilter<TdbCAPSubscribeFilter>();
            if (assemblies is not null && assemblies.Count > 0)
            {
                capBuilder.AddSubscriberAssembly(assemblies.ToArray());
            }

            return capBuilder;
        }
    }
}
