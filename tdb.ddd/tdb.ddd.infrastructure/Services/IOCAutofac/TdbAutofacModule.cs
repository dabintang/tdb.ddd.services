using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.Scanning;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// autofac注册模块
    /// </summary>
    public class TdbAutofacModule : Autofac.Module
    {
        /// <summary>
        /// 重写Autofac管道Load方法，在这里注册注入
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            //获取需要注册的程序集集合
            var lstAssembly = this.GetRegisterAssemblys();

            //拦截器接口
            var iIntercept = typeof(ITdbIOCIntercept);

            //Scoped
            this.RegisterAssemblyTypes(builder, typeof(ITdbIOCScoped), (builder) =>
            {
                return builder.InstancePerLifetimeScope();
            });

            //Singleton
            this.RegisterAssemblyTypes(builder, typeof(ITdbIOCSingleton), (builder) =>
            {
                return builder.SingleInstance();
            });

            //Transient
            this.RegisterAssemblyTypes(builder, typeof(ITdbIOCTransient), (builder) =>
            {
                return builder.InstancePerDependency();
            });

            //注入缓存拦截器
            builder.Register(c => new TdbCacheInterceptor()).SingleInstance();

            #region 一个接口多处实现时注册

            ////单独注册
            //builder.RegisterType<WxPayService>().Named<IPayService>(typeof(WxPayService).Name);
            //builder.RegisterType<AliPayService>().Named<IPayService>(typeof(AliPayService).Name);

            #endregion
        }

        /// <summary>
        /// 通过程序集注册
        /// </summary>
        /// <param name="builder">容器创建器</param>
        /// <param name="type">基类</param>
        /// <param name="funcInstance">注册方法</param>
        private void RegisterAssemblyTypes(ContainerBuilder builder, Type type,
            Func<IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>, IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>> funcInstance)
        {
            //拦截器接口
            var iIntercept = typeof(ITdbIOCIntercept);
            //获取需要注册的程序集集合
            var lstAssembly = this.GetRegisterAssemblys();
            foreach (var assembly in lstAssembly)
            {
                //注册程序集中的对象（不加拦截器）
                var noIntercept = builder.RegisterAssemblyTypes(assembly).Where(m => type.IsAssignableFrom(m) && m != type).AsImplementedInterfaces();
                funcInstance(noIntercept);

                //注册程序集中的对象（加拦截器）
                var hadIntercept = builder.RegisterAssemblyTypes(assembly).Where(m => type.IsAssignableFrom(m) && m != type && iIntercept.IsAssignableFrom(iIntercept) && m != iIntercept)
                                          .AsImplementedInterfaces()//表示注册的类型，以接口的方式注册
                                          .EnableInterfaceInterceptors();//引用Autofac.Extras.DynamicProxy,使用接口的拦截器，在使用特性 [Attribute] 注册时，注册拦截器可注册到接口(Interface)上或其实现类(Implement)上。使用注册到接口上方式，所有的实现类都能应用到拦截器。;
                funcInstance(hadIntercept);
            }
        }

        /// <summary>
        /// 获取需要注册的程序集名称集合
        /// （默认获取所有tdb.开头 .dll结尾的程序集：tdb.*.dll）
        /// </summary>
        /// <returns></returns>
        protected virtual List<Assembly> GetRegisterAssemblys()
        {
            var list = new List<Assembly>();
            var dllFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "tdb.*.dll", SearchOption.AllDirectories);
            foreach (var dllFileName in dllFileNames)
            {
                list.Add(Assembly.LoadFrom(dllFileName));
            }

            return list;
        }
    }

    #region 一个接口多处实现时使用

    //public class HomeController : Controller
    //{
    //    private IPersonService _personService;
    //    private IPayService _wxPayService;
    //    private IPayService _aliPayService;
    //    private IComponentContext _componentContext;//Autofac上下文
    //    //通过构造函数注入Service
    //    public HomeController(IPersonService personService, IComponentContext componentContext)
    //    {
    //        _personService = personService;
    //        _componentContext = componentContext;
    //        //解释组件
    //        _wxPayService = _componentContext.ResolveNamed<IPayService>(typeof(WxPayService).Name);
    //        _aliPayService = _componentContext.ResolveNamed<IPayService>(typeof(AliPayService).Name);
    //    }
    //    public IActionResult Index()
    //    {
    //        ViewBag.eat = _personService.Eat();
    //        ViewBag.wxPay = _wxPayService.Pay();
    //        ViewBag.aliPay = _aliPayService.Pay();
    //        return View();
    //    }
    //}

    #endregion
}