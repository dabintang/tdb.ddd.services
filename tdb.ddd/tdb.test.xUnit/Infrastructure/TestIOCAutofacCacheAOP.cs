using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.test.xUnit.Infrastructure
{
    /// <summary>
    /// 测试Autofac拦截器做缓存
    /// </summary>
    public class TestIOCAutofacCacheAOP
    {
        /// <summary>
        /// 容器
        /// </summary>
        private readonly IContainer container;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TestIOCAutofacCacheAOP()
        {
            TdbCache.InitCache(new TdbMemoryCache());

            var builder = new ContainerBuilder();
            //new TestAutofacCacheAOPModule().Configure(builder.ComponentRegistryBuilder);
            new TdbAutofacModule().Configure(builder.ComponentRegistryBuilder);
            this.container = builder.Build();
        }

        /// <summary>
        /// 测试无缓存
        /// </summary>
        [Fact]
        public void TestNoCache()
        {
            var obj = this.container.Resolve<IClassScopedCacheAOP>();
            var value1 = "value1";
            var value2 = "value2";

            //无缓存
            var noCacheValue1 = obj.NoCache(value1);
            Assert.Equal(noCacheValue1, value1);
            var noCacheValue2 = obj.NoCache(value2);
            Assert.Equal(noCacheValue2, value2);
        }

        /// <summary>
        /// 测试string型缓存
        /// </summary>
        [Fact]
        public void TestCacheString()
        {
            var obj = this.container.Resolve<IClassScopedCacheAOP>();
            var value1 = "value1";
            var value2 = "value2";
            var key = "key";

            //缓存
            var cacheValue1 = obj.ReadCacheString(value1, key);
            Assert.Equal(cacheValue1, value1);
            var cacheValue2 = obj.ReadCacheString(value2, key);
            Assert.Equal(cacheValue2, value1);
            //删除缓存
            obj.RemoveCacheString(key);
            var cacheValue3 = obj.ReadCacheString(value2, key);
            Assert.Equal(cacheValue3, value2);
            var cacheValue4 = obj.ReadCacheString(value1, key);
            Assert.Equal(cacheValue4, value2);
        }

        /// <summary>
        /// 测试hash型缓存
        /// </summary>
        [Fact]
        public void TestCacheHash()
        {
            var obj = this.container.Resolve<IClassScopedCacheAOP>();
            var info11 = new TestIOCAutofacCacheAOPInfo() { ID = 1, Name = "名字1" };
            var info12 = new TestIOCAutofacCacheAOPInfo() { ID = 1, Name = "名字2" };

            //缓存
            var cacheValue1 = obj.ReadCacheHash(info11);
            AssertClassEqual(cacheValue1, info11);
            var cacheValue2 = obj.ReadCacheHash(info12);
            AssertClassEqual(cacheValue2, info11);
            //删除缓存
            obj.RemoveCacheHash(info11.ID);
            var cacheValue3 = obj.ReadCacheHash(info12);
            AssertClassEqual(cacheValue3, info12);
            var cacheValue4 = obj.ReadCacheHash(info11);
            AssertClassEqual(cacheValue4, info12);

            var info21 = new TestIOCAutofacCacheAOPInfo() { ID = 2, Name = "名字1" };
            var cacheValue5 = obj.ReadCacheHash(info21);
            AssertClassEqual(cacheValue5, info21);
        }

        /// <summary>
        /// 断言2个对象内容一致
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        private static void AssertClassEqual(object obj1, object obj2)
        {
            var json1 = obj1.SerializeJson();
            var json2 = obj2.SerializeJson();
            Assert.Equal(json1, json2);
        }
    }

    ///// <summary>
    ///// 注册模块
    ///// </summary>
    //public class TestAutofacCacheAOPModule : TdbAutofacModule
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

    /// <summary>
    /// 测试接口
    /// </summary>
    public interface IClassScopedCacheAOP : ITdbIOCScoped, ITdbIOCIntercept
    {
        /// <summary>
        /// 无缓存直接返回入参
        /// </summary>
        /// <param name="input">入参</param>
        /// <returns></returns>
        string NoCache(string input);

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="input">入参</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        string ReadCacheString(string input, string key);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        void RemoveCacheString(string key);

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        TestIOCAutofacCacheAOPInfo ReadCacheHash(TestIOCAutofacCacheAOPInfo info);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">key</param>
        /// <returns></returns>
        void RemoveCacheHash(int id);
    }

    /// <summary>
    /// 测试类
    /// </summary>
    [Intercept(typeof(TdbCacheInterceptor))]
    public class ClassScopedCacheAOP : IClassScopedCacheAOP
    {
        /// <summary>
        /// 无缓存直接返回入参
        /// </summary>
        /// <param name="input">入参</param>
        /// <returns></returns>
        public string NoCache(string input)
        {
            return input;
        }

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="input">入参</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        [TdbReadCacheString("StringKeyPrefix")]
        [TdbCacheKey(1)]
        public string ReadCacheString(string input, string key)
        {
            return input;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        [TdbRemoveCacheString("StringKeyPrefix")]
        [TdbCacheKey(0)]
        public void RemoveCacheString(string key)
        {
        }

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        [TdbReadCacheHash("HashKey")]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public TestIOCAutofacCacheAOPInfo ReadCacheHash(TestIOCAutofacCacheAOPInfo info)
        {
            return info;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">key</param>
        /// <returns></returns>
        [TdbRemoveCacheHash("HashKey")]
        [TdbCacheKey(0)]
        public void RemoveCacheHash(int id)
        {
        }
    }

    /// <summary>
    /// 模拟一些信息类
    /// </summary>
    public class TestIOCAutofacCacheAOPInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
