using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
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
    public class TestIOCAutofacCacheAOP_Redis
    {
        /// <summary>
        /// 容器
        /// </summary>
        private readonly IContainer container;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TestIOCAutofacCacheAOP_Redis()
        {
            //TdbCache.InitCache(new TdbMemoryCache());
            TdbCache.InitCache(new TdbRedisCache("127.0.0.1,defaultDatabase=0,idleTimeout=30000,poolsize=10,prefix=BaseTest_"));

            var builder = new ContainerBuilder();
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
            var key = "keyCacheString";

            //删除缓存
            obj.RemoveCacheString(key);

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
        /// 测试string型缓存
        /// </summary>
        [Fact]
        public async Task TestCacheStringAsync()
        {
            var obj = this.container.Resolve<IClassScopedCacheAOP>();
            var info11 = new TestIOCAutofacCacheAOPInfo() { ID = 1, Name = "名字1" };
            var info12 = new TestIOCAutofacCacheAOPInfo() { ID = 1, Name = "名字2" };

            //删除缓存
            await obj.RemoveCacheStringAsync(info11.ID);

            //缓存
            var cacheValue1 = await obj.ReadCacheStringAsync(info11);
            AssertClassEqual(cacheValue1, info11);
            var cacheValue2 = await obj.ReadCacheStringAsync(info12);
            AssertClassEqual(cacheValue2, info11);
            //删除缓存
            await obj.RemoveCacheStringAsync(info11.ID);
            var cacheValue3 = await obj.ReadCacheStringAsync(info12);
            AssertClassEqual(cacheValue3, info12);
            var cacheValue4 = await obj.ReadCacheStringAsync(info11);
            AssertClassEqual(cacheValue4, info12);

            var info21 = new TestIOCAutofacCacheAOPInfo() { ID = 2, Name = "名字1" };

            //删除缓存
            await obj.RemoveCacheStringAsync(info21.ID);

            var cacheValue5 = await obj.ReadCacheStringAsync(info21);
            AssertClassEqual(cacheValue5, info21);
        }

        /// <summary>
        /// 测试hash型缓存
        /// </summary>
        [Fact]
        public void TestCacheHash()
        {
            var obj = this.container.Resolve<IClassScopedCacheAOP>();
            var value1 = "value1";
            var value2 = "value2";
            var field = "fieldCacheHash";

            //删除缓存
            obj.RemoveCacheHash(field);

            //缓存
            var cacheValue1 = obj.ReadCacheHash(value1, field);
            Assert.Equal(cacheValue1, value1);
            var cacheValue2 = obj.ReadCacheHash(value2, field);
            Assert.Equal(cacheValue2, value1);
            //删除缓存
            obj.RemoveCacheHash(field);
            var cacheValue3 = obj.ReadCacheHash(value2, field);
            Assert.Equal(cacheValue3, value2);
            var cacheValue4 = obj.ReadCacheHash(value1, field);
            Assert.Equal(cacheValue4, value2);
        }

        /// <summary>
        /// 测试hash型缓存
        /// </summary>
        [Fact]
        public void TestCacheHash2()
        {
            var obj = this.container.Resolve<IClassScopedCacheAOP>();
            var value1 = "value1";
            var value2 = "value2";
            var key = 1;
            var field = "fieldCacheHash2";

            //删除缓存
            obj.RemoveCacheHash2(key, field);

            //缓存
            var cacheValue1 = obj.ReadCacheHash2(value1, key, field);
            Assert.Equal(cacheValue1, value1);
            var cacheValue2 = obj.ReadCacheHash2(value2, key, field);
            Assert.Equal(cacheValue2, value1);
            //删除缓存
            obj.RemoveCacheHash2(key, field);
            var cacheValue3 = obj.ReadCacheHash2(value2, key, field);
            Assert.Equal(cacheValue3, value2);
            var cacheValue4 = obj.ReadCacheHash2(value1, key, field);
            Assert.Equal(cacheValue4, value2);
        }

        /// <summary>
        /// 测试hash型缓存
        /// </summary>
        [Fact]
        public async Task TestCacheHashAsync()
        {
            var obj = this.container.Resolve<IClassScopedCacheAOP>();
            var info11 = new TestIOCAutofacCacheAOPInfo() { ID = 1, Name = "名字1" };
            var info12 = new TestIOCAutofacCacheAOPInfo() { ID = 1, Name = "名字2" };

            //删除缓存
            await obj.RemoveCacheHashAsync(info11.ID);

            //缓存
            var cacheValue1 = await obj.ReadCacheHashAsync(info11);
            AssertClassEqual(cacheValue1, info11);
            var cacheValue2 = await obj.ReadCacheHashAsync(info12);
            AssertClassEqual(cacheValue2, info11);
            //删除缓存
            await obj.RemoveCacheHashAsync(info11.ID);
            var cacheValue3 = await obj.ReadCacheHashAsync(info12);
            AssertClassEqual(cacheValue3, info12);
            var cacheValue4 = await obj.ReadCacheHashAsync(info11);
            AssertClassEqual(cacheValue4, info12);

            var info21 = new TestIOCAutofacCacheAOPInfo() { ID = 2, Name = "名字1" };
            var info22 = new TestIOCAutofacCacheAOPInfo() { ID = 2, Name = "名字2" };

            //删除缓存
            await obj.RemoveCacheHashAsync(info21.ID);

            var cacheValue5 = await obj.ReadCacheHashAsync(info21);
            AssertClassEqual(cacheValue5, info21);
            var cacheValue6 = await obj.ReadCacheHashAsync(info22);
            AssertClassEqual(cacheValue6, info21);
        }

        /// <summary>
        /// 测试hash型缓存
        /// </summary>
        [Fact]
        public async Task TestCacheHash2Async()
        {
            var obj = this.container.Resolve<IClassScopedCacheAOP>();
            var info11 = new TestIOCAutofacCacheAOPInfo() { ID = 1, Name = "名字1" };
            var info12 = new TestIOCAutofacCacheAOPInfo() { ID = 1, Name = "名字2" };

            //删除缓存
            await obj.RemoveCacheHash2Async(info11);

            //缓存
            var cacheValue1 = await obj.ReadCacheHash2Async(info11);
            AssertClassEqual(cacheValue1, info11);
            var cacheValue2 = await obj.ReadCacheHash2Async(info12);
            AssertClassEqual(cacheValue2, info11);
            //删除缓存
            await obj.RemoveCacheHash2Async(info11);
            var cacheValue3 = await obj.ReadCacheHash2Async(info12);
            AssertClassEqual(cacheValue3, info12);
            var cacheValue4 = await obj.ReadCacheHash2Async(info11);
            AssertClassEqual(cacheValue4, info12);

            var info21 = new TestIOCAutofacCacheAOPInfo() { ID = 2, Name = "名字1" };
            var info22 = new TestIOCAutofacCacheAOPInfo() { ID = 2, Name = "名字2" };

            //删除缓存
            await obj.RemoveCacheHash2Async(info21);

            var cacheValue5 = await obj.ReadCacheHash2Async(info21);
            AssertClassEqual(cacheValue5, info21);
            var cacheValue6 = await obj.ReadCacheHash2Async(info22);
            AssertClassEqual(cacheValue6, info21);
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
        string ReadCacheString(string input, string field);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        void RemoveCacheString(string field);

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        Task<TestIOCAutofacCacheAOPInfo> ReadCacheStringAsync(TestIOCAutofacCacheAOPInfo info);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">key</param>
        /// <returns></returns>
        Task RemoveCacheStringAsync(int id);

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="input">入参</param>
        /// <param name="field">field</param>
        /// <returns></returns>
        string ReadCacheHash(string input, string field);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="field">field</param>
        /// <returns></returns>
        void RemoveCacheHash(string field);

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="input">入参</param>
        /// <param name="key">key</param>
        /// <param name="field">field</param>
        /// <returns></returns>
        string ReadCacheHash2(string input, int key, string field);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">field</param>
        /// <returns></returns>
        void RemoveCacheHash2(int key, string field);

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        Task<TestIOCAutofacCacheAOPInfo> ReadCacheHashAsync(TestIOCAutofacCacheAOPInfo info);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">key</param>
        /// <returns></returns>
        Task RemoveCacheHashAsync(int id);

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        Task<TestIOCAutofacCacheAOPInfo> ReadCacheHash2Async(TestIOCAutofacCacheAOPInfo info);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        Task RemoveCacheHash2Async(TestIOCAutofacCacheAOPInfo info);
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
        [TdbReadCacheString("CacheString")]
        [TdbCacheKey(1)]
        public string ReadCacheString(string input, string field)
        {
            return input;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        [TdbRemoveCacheString("CacheString")]
        [TdbCacheKey(0)]
        public void RemoveCacheString(string field)
        {
        }

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        [TdbReadCacheString("CacheStringAsync")]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task<TestIOCAutofacCacheAOPInfo> ReadCacheStringAsync(TestIOCAutofacCacheAOPInfo info)
        {
            await Task.CompletedTask;
            return info;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">key</param>
        /// <returns></returns>
        [TdbRemoveCacheString("CacheStringAsync")]
        [TdbCacheKey(0)]
        public async Task RemoveCacheStringAsync(int id)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="input">入参</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        [TdbReadCacheHash("CacheHash")]
        [TdbCacheKey(1)]
        public string ReadCacheHash(string input, string field)
        {
            return input;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="field">key</param>
        /// <returns></returns>
        [TdbRemoveCacheHash("CacheHash")]
        [TdbCacheKey(0)]
        public void RemoveCacheHash(string field)
        {
        }

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="input">入参</param>
        /// <param name="key">key</param>
        /// <param name="field">field</param>
        /// <returns></returns>
        [TdbReadCacheHash("CacheHash2", ParamIndex = 1)]
        [TdbCacheKey(2)]
        public string ReadCacheHash2(string input, int key, string field)
        {
            return input;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">field</param>
        /// <returns></returns>
        [TdbRemoveCacheHash("CacheHash2", ParamIndex = 0)]
        [TdbCacheKey(1)]
        public void RemoveCacheHash2(int key, string field)
        {
        }

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        [TdbReadCacheHash("CacheHashAsync")]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task<TestIOCAutofacCacheAOPInfo> ReadCacheHashAsync(TestIOCAutofacCacheAOPInfo info)
        {
            await Task.CompletedTask;
            return info;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">key</param>
        /// <returns></returns>
        [TdbRemoveCacheHash("CacheHashAsync")]
        [TdbCacheKey(0)]
        public async Task RemoveCacheHashAsync(int id)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 优先从缓存返回，如无缓存则返回入参并加入缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        [TdbReadCacheHash("CacheHash2Async", ParamIndex = 0, FromPropertyName = "TypeCode")]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task<TestIOCAutofacCacheAOPInfo> ReadCacheHash2Async(TestIOCAutofacCacheAOPInfo info)
        {
            await Task.CompletedTask;
            return info;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="info">入参</param>
        /// <returns></returns>
        [TdbRemoveCacheHash("CacheHash2Async", ParamIndex = 0, FromPropertyName = "TypeCode")]
        [TdbCacheKey(0, FromPropertyName = "ID")]
        public async Task RemoveCacheHash2Async(TestIOCAutofacCacheAOPInfo info)
        {
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// 模拟一些信息类
    /// </summary>
    public class TestIOCAutofacCacheAOPInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string TypeCode { get; set; } = "A";

        /// <summary>
        /// 
        /// </summary>
        [JsonInclude]
        public string Name { get; internal set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string ReadOnlyName { get { return GetName(); } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return Name;
        }
    }
}
