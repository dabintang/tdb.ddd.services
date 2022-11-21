using Autofac;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.test.xUnit.Infrastructure
{
    /// <summary>
    /// 测试Autofac
    /// </summary>
    public class TestIOCAutofac
    {
        /// <summary>
        /// 容器
        /// </summary>
        private readonly IContainer container;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TestIOCAutofac()
        {
            var builder = new ContainerBuilder();
            //new TestAutofacModule().Configure(builder.ComponentRegistryBuilder);
            new TdbAutofacModule().Configure(builder.ComponentRegistryBuilder);

            this.container = builder.Build();
        }

        /// <summary>
        /// 测试Scoped生命周期的类
        /// </summary>
        [Fact]
        public void TestScoped()
        {
            var objScoped = this.container.Resolve<IClassScoped>();
            Assert.NotNull(objScoped);

            var objScoped2 = this.container.Resolve<IClassScoped>();
            Assert.Equal(objScoped, objScoped2);
        }

        /// <summary>
        /// 测试Singleton生命周期的类
        /// </summary>
        [Fact]
        public void TestSingleton()
        {
            var objSingleton = this.container.Resolve<IClassSingleton>();
            Assert.NotNull(objSingleton);

            var objSingleton2 = this.container.Resolve<IClassSingleton>();
            Assert.Equal(objSingleton, objSingleton2);
        }

        /// <summary>
        /// 测试Transient生命周期的类
        /// </summary>
        [Fact]
        public void TestTransient()
        {
            var objTransient = this.container.Resolve<IClassTransient>();
            Assert.NotNull(objTransient);

            var objTransient2 = this.container.Resolve<IClassTransient>();
            Assert.NotEqual(objTransient, objTransient2);
        }
    }

    ///// <summary>
    ///// 注册模块
    ///// </summary>
    //public class TestAutofacModule : TdbAutofacModule
    //{
    //    /// <summary>
    //    /// 获取需要注册的程序集名称集合
    //    /// </summary>
    //    /// <returns></returns>
    //    /// <exception cref="NotImplementedException"></exception>
    //    protected override List<Assembly> GetRegisterAssemblys()
    //    {
    //        return new List<Assembly>() { Assembly.GetExecutingAssembly() };
    //    }
    //}

    public interface IClassScoped : ITdbIOCScoped { }
    /// <summary>
    /// Scoped生命周期的类
    /// </summary>
    public class ClassScoped : IClassScoped
    {
    }

    public interface IClassSingleton : ITdbIOCSingleton { }
    /// <summary>
    /// Singleton生命周期的类
    /// </summary>
    public class ClassSingleton : IClassSingleton
    {
    }

    public interface IClassTransient : ITdbIOCTransient { }
    /// <summary>
    /// Transient生命周期的类
    /// </summary>
    public class ClassTransient : IClassTransient
    {
    }
}
