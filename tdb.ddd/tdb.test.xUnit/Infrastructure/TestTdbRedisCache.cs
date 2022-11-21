using tdb.common;
using tdb.ddd.infrastructure.Services;

namespace tdb.test.xUnit.Infrastructure
{
    /// <summary>
    /// 测试Redis缓存
    /// </summary>
    public class TestTdbRedisCache
    {
        private readonly TdbRedisCache cache = new("127.0.0.1,defaultDatabase=0,idleTimeout=30000,poolsize=10,prefix=BaseTest_");

        /// <summary>
        /// 测试Set方法
        /// </summary>
        [Fact]
        public void TestSet()
        {
            //string
            this.TestSetInner("string缓存值");

            //int
            this.TestSetInner(123);

            //int?
            this.TestSetInner<int?>(456);
            this.TestSetInner<int?>(null);

            //decimal
            this.TestSetInner(123.456M);

            //decimal?
            this.TestSetInner<decimal?>(456.789M);
            this.TestSetInner<decimal?>(null);

            //DateTime
            this.TestSetInner(DateTime.Now);

            //DateTime?
            this.TestSetInner<DateTime?>(DateTime.Now);
            this.TestSetInner<DateTime?>(null);

            //enum
            this.TestSetInner(EnmGender.Male);

            //enum?
            this.TestSetInner<EnmGender?>(EnmGender.Male);
            this.TestSetInner<EnmGender?>(null);

            #region class

            var cVal = new ClassOjbect
            {
                Text = null,
                Age = 1,
                GenderCode = EnmGender.Female,
                C2 = new ClassOjbect2()
            };
            cVal.C2.Text2 = "C2";
            cVal.C2.Age2 = 2;
            cVal.S2.Text2 = "S2";
            cVal.S2.Age2 = 2;
            this.TestSetInner(cVal);

            #endregion

            #region list<string>

            var list = new List<string>() { "A", "b", "C" };
            this.TestSetInner(list);

            #endregion

            #region Dictionary<string, int>

            var dic = new Dictionary<string, int>
            {
                { "A", 1 },
                { "B", 2 }
            };
            this.TestSetInner(dic);

            #endregion
        }

        /// <summary>
        /// 测试Set方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">缓存内容</param>
        /// <returns>缓存key</returns>
        private string TestSetInner<T>(T value)
        {
            var key = Guid.NewGuid().ToString();

            //设置缓存
            this.cache.Set(key, value, TimeSpan.FromMinutes(1));
            //获取缓存
            var cacheValue = this.cache.Get<T>(key);
            //设置的值和获取的值应一致
            if (typeof(T).IsClass)
            {
                var json1 = value.SerializeJson();
                var json2 = cacheValue.SerializeJson();
                Assert.Equal(json1, json2);
            }
            else if (value is DateTime)
            {
                var json1 = value.ToString();
                var json2 = cacheValue.ToString();
                Assert.Equal(json1, json2);
            }
            else
            {
                Assert.Equal(value, cacheValue);
            }

            return key;
        }

        /// <summary>
        /// 测试SetAsync方法
        /// </summary>
        [Fact]
        public async Task TestSetAsync()
        {
            //string
            await this.TestSetAsyncInner("string缓存值");

            //int
            await this.TestSetAsyncInner(123);

            //int?
            await this.TestSetAsyncInner<int?>(456);
            await this.TestSetAsyncInner<int?>(null);

            //decimal
            await this.TestSetAsyncInner(123.456M);

            //decimal?
            await this.TestSetAsyncInner<decimal?>(456.789M);
            await this.TestSetAsyncInner<decimal?>(null);

            //DateTime
            await this.TestSetAsyncInner(DateTime.Now);

            //DateTime?
            await this.TestSetAsyncInner<DateTime?>(DateTime.Now);
            await this.TestSetAsyncInner<DateTime?>(null);

            //enum
            await this.TestSetAsyncInner(EnmGender.Male);

            //enum?
            await this.TestSetAsyncInner<EnmGender?>(EnmGender.Male);
            await this.TestSetAsyncInner<EnmGender?>(null);

            #region class

            var cVal = new ClassOjbect
            {
                Text = null,
                Age = 1,
                GenderCode = EnmGender.Female,
                C2 = new ClassOjbect2()
            };
            cVal.C2.Text2 = "C2";
            cVal.C2.Age2 = 2;
            cVal.S2.Text2 = "S2";
            cVal.S2.Age2 = 2;
            await this.TestSetAsyncInner(cVal);

            #endregion

            #region list<string>

            var list = new List<string>() { "A", "b", "C" };
            await this.TestSetAsyncInner(list);

            #endregion

            #region Dictionary<string, int>

            var dic = new Dictionary<string, int>
            {
                { "A", 1 },
                { "B", 2 }
            };
            await this.TestSetAsyncInner(dic);

            #endregion
        }

        /// <summary>
        /// 测试SetAsync方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">缓存内容</param>
        /// <returns>缓存key</returns>
        private async Task<string> TestSetAsyncInner<T>(T value)
        {
            var key = Guid.NewGuid().ToString();

            //设置缓存
            await this.cache.SetAsync(key, value, TimeSpan.FromMinutes(1));
            //获取缓存
            var cacheValue = this.cache.Get<T>(key);
            //设置的值和获取的值应一致
            if (typeof(T).IsClass)
            {
                var json1 = value.SerializeJson();
                var json2 = cacheValue.SerializeJson();
                Assert.Equal(json1, json2);
            }
            else if (value is DateTime)
            {
                var json1 = value.ToString();
                var json2 = cacheValue.ToString();
                Assert.Equal(json1, json2);
            }
            else
            {
                Assert.Equal(value, cacheValue);
            }

            return key;
        }

        /// <summary>
        /// 测试Del方法
        /// </summary>
        [Fact]
        public void TestDel()
        {
            //设置缓存
            this.cache.Set("keyTestDel1", 1, TimeSpan.FromMinutes(1));
            this.cache.Set("keyTestDel2", "2", TimeSpan.FromMinutes(1));
            //删除缓存
            this.cache.Del("keyTestDel1", "keyTestDel2");
            //获取缓存
            var val1 = this.cache.Get<int?>("keyTestDel1");
            var val2 = this.cache.Get<string>("keyTestDel2");

            Assert.True(val1 == null);
            Assert.True(val2 == null);
        }

        /// <summary>
        /// 测试DelAsync方法
        /// </summary>
        [Fact]
        public async Task TestDelAsync()
        {
            //设置缓存
            this.cache.Set("keyTestDelAsync1", 1, TimeSpan.FromMinutes(1));
            this.cache.Set("keyTestDelAsync2", "2", TimeSpan.FromMinutes(1));
            //删除缓存
            await this.cache.DelAsync("keyTestDelAsync1", "keyTestDelAsync2");
            //获取缓存
            var val1 = this.cache.Get<int?>("keyTestDelAsync1");
            var val2 = this.cache.Get<string>("keyTestDelAsync2");

            Assert.True(val1 == null);
            Assert.True(val2 == null);
        }

        /// <summary>
        /// 测试Exists方法
        /// </summary>
        [Fact]
        public void TestExists()
        {
            //设置缓存
            this.cache.Set("keyTestExists1", 1, TimeSpan.FromMinutes(1));
            this.cache.Set("keyTestExists2", "2", TimeSpan.FromMinutes(1));
            //删除缓存
            this.cache.Del("keyTestExists2");
            //判断key是否存在
            var val1 = this.cache.Exists("keyTestExists1");
            var val2 = this.cache.Exists("keyTestExists2");

            Assert.True(val1 == true);
            Assert.True(val2 == false);
        }

        /// <summary>
        /// 测试Expire方法
        /// </summary>
        [Fact]
        public void TestExpire()
        {
            //设置缓存
            this.cache.Set("keyTestExpire1", 1, TimeSpan.FromSeconds(10));

            //设置过期时间
            this.cache.Expire("keyTestExpire1", TimeSpan.FromSeconds(1));

            //判断key是否存在
            var val1 = this.cache.Exists("keyTestExpire1");
            Assert.True(val1 == true);

            Thread.Sleep(1001);
            //判断key是否存在
            var val2 = this.cache.Exists("keyTestExpire1");
            Assert.True(val2 == false);
        }

        /// <summary>
        /// 测试ExpireAsync方法
        /// </summary>
        [Fact]
        public async Task TestExpireAsync()
        {
            //设置缓存
            this.cache.Set("keyTestExpireAsync1", 1, TimeSpan.FromSeconds(10));

            //设置过期时间
            await this.cache.ExpireAsync("keyTestExpireAsync1", TimeSpan.FromSeconds(1));

            //判断key是否存在
            var val1 = this.cache.Exists("keyTestExpireAsync1");
            Assert.True(val1 == true);

            Thread.Sleep(1001);
            //判断key是否存在
            var val2 = this.cache.Exists("keyTestExpireAsync1");
            Assert.True(val2 == false);
        }

        /// <summary>
        /// 测试ExpireAt方法
        /// </summary>
        [Fact]
        public void TestExpireAt()
        {
            //设置缓存
            this.cache.Set("keyTestExpireAt1", 1, TimeSpan.FromSeconds(10));
            //设置过期时间点
            this.cache.ExpireAt("keyTestExpireAt1", DateTime.Now.AddSeconds(1));

            //判断key是否存在
            var val1 = this.cache.Exists("keyTestExpireAt1");
            Assert.True(val1 == true);

            Thread.Sleep(1001);
            //判断key是否存在
            var val2 = this.cache.Exists("keyTestExpireAt1");
            Assert.True(val2 == false);
        }

        /// <summary>
        /// 测试ExpireAtAsync方法
        /// </summary>
        [Fact]
        public async Task TestExpireAtAsync()
        {
            //设置缓存
            this.cache.Set("keyTestExpireAtAsync1", 1, TimeSpan.FromSeconds(10));
            //设置过期时间点
            await this.cache.ExpireAtAsync("keyTestExpireAtAsync1", DateTime.Now.AddSeconds(1));

            //判断key是否存在
            var val1 = this.cache.Exists("keyTestExpireAtAsync1");
            Assert.True(val1 == true);

            Thread.Sleep(1001);
            //判断key是否存在
            var val2 = this.cache.Exists("keyTestExpireAtAsync1");
            Assert.True(val2 == false);
        }

        /// <summary>
        /// 测试HSet方法
        /// </summary>
        [Fact]
        public void TestHSet()
        {
            //string
            this.TestHSetInner("string缓存值");

            //int
            this.TestHSetInner(123);

            //int?
            this.TestHSetInner<int?>(456);
            this.TestHSetInner<int?>(null);

            //decimal
            this.TestHSetInner(123.456M);

            //decimal?
            this.TestHSetInner<decimal?>(456.789M);
            this.TestHSetInner<decimal?>(null);

            //DateTime
            this.TestHSetInner(DateTime.Now);

            //DateTime?
            this.TestHSetInner<DateTime?>(DateTime.Now);
            this.TestHSetInner<DateTime?>(null);

            //enum
            this.TestHSetInner(EnmGender.Male);

            //enum?
            this.TestHSetInner<EnmGender?>(EnmGender.Male);
            this.TestHSetInner<EnmGender?>(null);

            #region class

            var cVal = new ClassOjbect
            {
                Text = null,
                Age = 1,
                GenderCode = EnmGender.Female,
                C2 = new ClassOjbect2()
            };
            cVal.C2.Text2 = "C2";
            cVal.C2.Age2 = 2;
            cVal.S2.Text2 = "S2";
            cVal.S2.Age2 = 2;
            this.TestHSetInner(cVal);

            #endregion

            #region list<string>

            var list = new List<string>() { "A", "b", "C" };
            this.TestHSetInner(list);

            #endregion

            #region Dictionary<string, int>

            var dic = new Dictionary<string, int>
            {
                { "A", 1 },
                { "B", 2 }
            };
            this.TestHSetInner(dic);

            #endregion
        }

        /// <summary>
        /// 测试HSet方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">缓存内容</param>
        /// <returns>缓存key</returns>
        private string TestHSetInner<T>(T value)
        {
            var key = Guid.NewGuid().ToString();
            var field = "field1";

            //设置缓存
            this.cache.HSet(key, field, value);
            //设置过期时间
            this.cache.Expire(key, TimeSpan.FromMinutes(1));
            //获取缓存
            var cacheValue = this.cache.HGet<T>(key, field);
            //设置的值和获取的值应一致
            if (typeof(T).IsClass)
            {
                var json1 = value.SerializeJson();
                var json2 = cacheValue.SerializeJson();
                Assert.Equal(json1, json2);
            }
            else if (value is DateTime)
            {
                var json1 = value.ToString();
                var json2 = cacheValue.ToString();
                Assert.Equal(json1, json2);
            }
            else
            {
                Assert.Equal(value, cacheValue);
            }

            return key;
        }

        /// <summary>
        /// 测试HSetAsync方法
        /// </summary>
        [Fact]
        public async Task TestHSetAsync()
        {
            //string
            await this.TestHSetAsyncInner("string缓存值");

            //int
            await this.TestHSetAsyncInner(123);

            //int?
            await this.TestHSetAsyncInner<int?>(456);
            await this.TestHSetAsyncInner<int?>(null);

            //decimal
            await this.TestHSetAsyncInner(123.456M);

            //decimal?
            await this.TestHSetAsyncInner<decimal?>(456.789M);
            await this.TestHSetAsyncInner<decimal?>(null);

            //DateTime
            await this.TestHSetAsyncInner(DateTime.Now);

            //DateTime?
            await this.TestHSetAsyncInner<DateTime?>(DateTime.Now);
            await this.TestHSetAsyncInner<DateTime?>(null);

            //enum
            await this.TestHSetAsyncInner(EnmGender.Male);

            //enum?
            await this.TestHSetAsyncInner<EnmGender?>(EnmGender.Male);
            await this.TestHSetAsyncInner<EnmGender?>(null);

            #region class

            var cVal = new ClassOjbect
            {
                Text = null,
                Age = 1,
                GenderCode = EnmGender.Female,
                C2 = new ClassOjbect2()
            };
            cVal.C2.Text2 = "C2";
            cVal.C2.Age2 = 2;
            cVal.S2.Text2 = "S2";
            cVal.S2.Age2 = 2;
            await this.TestHSetAsyncInner(cVal);

            #endregion

            #region list<string>

            var list = new List<string>() { "A", "b", "C" };
            await this.TestHSetAsyncInner(list);

            #endregion

            #region Dictionary<string, int>

            var dic = new Dictionary<string, int>
            {
                { "A", 1 },
                { "B", 2 }
            };
            await this.TestHSetAsyncInner(dic);

            #endregion
        }

        /// <summary>
        /// 测试HSetAsync方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">缓存内容</param>
        /// <returns>缓存key</returns>
        private async Task<string> TestHSetAsyncInner<T>(T value)
        {
            var key = Guid.NewGuid().ToString();
            var field = "field1";

            //设置缓存
            await this.cache.HSetAsync(key, TimeSpan.FromMinutes(1), field, value);
            //获取缓存
            var cacheValue = this.cache.HGet<T>(key, field);
            //设置的值和获取的值应一致
            if (typeof(T).IsClass)
            {
                var json1 = value.SerializeJson();
                var json2 = cacheValue.SerializeJson();
                Assert.Equal(json1, json2);
            }
            else if (value is DateTime)
            {
                var json1 = value.ToString();
                var json2 = cacheValue.ToString();
                Assert.Equal(json1, json2);
            }
            else
            {
                Assert.Equal(value, cacheValue);
            }

            return key;
        }

        /// <summary>
        /// 测试HMSet方法
        /// </summary>
        [Fact]
        public void TestHMSet()
        {
            var key = "TestHMSet";

            var value = new Dictionary<string, string>()
            {
                { "field1", "value1" },
                { "field2", "value2" },
                { "field3", "value3" }
            };
            var keyValues = value.Select(m => (m.Key, (object)m.Value));

            //设置缓存
            this.cache.HMSet(key, keyValues.ToArray());
            //设置过期时间
            this.cache.Expire(key, TimeSpan.FromMinutes(1));
            //获取缓存
            var cacheValue = this.cache.HGetAll<string>(key);
            //设置的值和获取的值应一致
            var json1 = value.SerializeJson();
            var json2 = cacheValue.SerializeJson();
            Assert.Equal(json1, json2);

            //修改cacheValue后应和原值不一致
            cacheValue["field4"] = "value4";
            json1 = value.SerializeJson();
            json2 = cacheValue.SerializeJson();
            Assert.NotEqual(json1, json2);

            //再设置一遍缓存
            this.cache.HMSet(key, keyValues.ToArray());
            //获取缓存
            cacheValue = this.cache.HGetAll<string>(key);
            //设置的值和获取的值应一致
            json1 = value.SerializeJson();
            json2 = cacheValue.SerializeJson();
            Assert.Equal(json1, json2);
        }

        /// <summary>
        /// 测试HMSetAsync方法
        /// </summary>
        [Fact]
        public async Task TestHMSetAsync()
        {
            var key = "TestHMSetAsync";

            var value = new Dictionary<string, string>()
            {
                { "field1", "value1" },
                { "field2", "value2" },
                { "field3", "value3" }
            };
            var keyValues = value.Select(m => (m.Key, (object)m.Value));

            //设置缓存
            await this.cache.HMSetAsync(key, TimeSpan.FromMinutes(1), keyValues.ToArray());
            //获取缓存
            var cacheValue = this.cache.HGetAll<string>(key);
            //设置的值和获取的值应一致
            var json1 = value.SerializeJson();
            var json2 = cacheValue.SerializeJson();
            Assert.Equal(json1, json2);

            //修改cacheValue后应和原值不一致
            cacheValue["field4"] = "value4";
            json1 = value.SerializeJson();
            json2 = cacheValue.SerializeJson();
            Assert.NotEqual(json1, json2);

            //再设置一遍缓存
            await this.cache.HMSetAsync(key, TimeSpan.FromMinutes(1), keyValues.ToArray());
            //获取缓存
            cacheValue = this.cache.HGetAll<string>(key);
            //设置的值和获取的值应一致
            json1 = value.SerializeJson();
            json2 = cacheValue.SerializeJson();
            Assert.Equal(json1, json2);
        }

        /// <summary>
        /// 测试HDel方法
        /// </summary>
        [Fact]
        public void TestHDel()
        {
            var key = "TestHDel";

            //设置缓存
            this.cache.HSet(key, "field1", 1);
            this.cache.HSet(key, "field2", 2);
            this.cache.HSet(key, "field3", 3);
            //设置过期时间
            this.cache.Expire(key, TimeSpan.FromMinutes(1));
            //删除缓存
            this.cache.HDel(key, "field1", "field3");
            //获取缓存
            var val1 = this.cache.HGet<int?>(key, "field1");
            var val2 = this.cache.HGet<int?>(key, "field2");
            var val3 = this.cache.HGet<int?>(key, "field3");

            Assert.Null(val1);
            Assert.True(val2 == 2);
            Assert.Null(val3);
        }

        /// <summary>
        /// 测试HDelAsync方法
        /// </summary>
        [Fact]
        public async Task TestHDelAsync()
        {
            var key = "TestHDelAsync";

            //设置缓存
            this.cache.HSet(key, "field1", 1);
            this.cache.HSet(key, "field2", 2);
            this.cache.HSet(key, "field3", 3);
            //设置过期时间
            this.cache.Expire(key, TimeSpan.FromMinutes(1));
            //删除缓存
            await this.cache.HDelAsync(key, "field1", "field3");
            //获取缓存
            var val1 = this.cache.HGet<int?>(key, "field1");
            var val2 = this.cache.HGet<int?>(key, "field2");
            var val3 = this.cache.HGet<int?>(key, "field3");

            Assert.Null(val1);
            Assert.True(val2 == 2);
            Assert.Null(val3);
        }

        /// <summary>
        /// 测试HExists方法
        /// </summary>
        [Fact]
        public void TestHExists()
        {
            var key = "TestHExists";

            //设置缓存
            this.cache.HMSet(key, ("field1", "value1"), ("field2", "value2"));
            Assert.True(this.cache.HExists(key, "field1"));
            Assert.True(this.cache.HExists(key, "field2"));
            Assert.False(this.cache.HExists(key, "field3"));

            //添加一个缓存
            this.cache.HSet(key, "field3", "value3");
            Assert.True(this.cache.HExists(key, "field1"));
            Assert.True(this.cache.HExists(key, "field2"));
            Assert.True(this.cache.HExists(key, "field3"));

            //删除缓存
            this.cache.HDel(key, "field1", "field3");
            Assert.False(this.cache.HExists(key, "field1"));
            Assert.True(this.cache.HExists(key, "field2"));
            Assert.False(this.cache.HExists(key, "field3"));

            //添加一个缓存
            this.cache.HMSet(key, ("field1", "value1"));
            Assert.True(this.cache.HExists(key, "field1"));
            Assert.True(this.cache.HExists(key, "field2"));
            Assert.False(this.cache.HExists(key, "field3"));

            this.cache.Expire(key, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 测试HKeys方法
        /// </summary>
        [Fact]
        public void TestHKeys()
        {
            var key = "TestHKeys";

            //设置缓存
            this.cache.HMSet(key, ("field1", "value1"), ("field2", "value2"));
            Assert.Equal(this.cache.HKeys(key).OrderBy(m => m).ToArray(), new String[] { "field1", "field2" });

            //添加一个缓存
            this.cache.HSet(key, "field3", "value3");
            Assert.Equal(this.cache.HKeys(key).OrderBy(m => m).ToArray(), new String[] { "field1", "field2", "field3" });

            //删除缓存
            this.cache.HDel(key, "field1", "field3");
            Assert.Equal(this.cache.HKeys(key).OrderBy(m => m).ToArray(), new String[] { "field2" });

            //添加一个缓存
            this.cache.HMSet(key, ("field1", "value1"));
            Assert.Equal(this.cache.HKeys(key).OrderBy(m => m).ToArray(), new String[] { "field1", "field2" });

            this.cache.Expire(key, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 测试HLen方法
        /// </summary>
        [Fact]
        public void TestHLen()
        {
            var key = "TestHLen";

            //设置缓存
            this.cache.HMSet(key, ("field1", "value1"), ("field2", "value2"));
            Assert.Equal(2L, this.cache.HLen(key));

            //添加一个缓存
            this.cache.HSet(key, "field3", "value3");
            Assert.Equal(3L, this.cache.HLen(key));

            //删除缓存
            this.cache.HDel(key, "field1", "field3");
            Assert.Equal(1L, this.cache.HLen(key));

            //添加一个缓存
            this.cache.HMSet(key, ("field1", "value1"));
            Assert.Equal(2L, this.cache.HLen(key));

            this.cache.Expire(key, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 测试CacheShell方法
        /// </summary>
        [Fact]
        public void TestCacheShell()
        {
            //已有缓存的情况
            var key1 = "TestCacheShell1";
            this.cache.Set(key1, "1", TimeSpan.FromMinutes(1));
            var cacheValue1 = this.cache.CacheShell<string>(key1, TimeSpan.FromSeconds(1), () => "2");
            Assert.Equal("1", cacheValue1);

            //未有缓存的情况
            var key2 = "TestCacheShell2";
            var cacheValue2 = this.cache.CacheShell<int?>(key2, TimeSpan.FromSeconds(1), () => 2);
            Assert.Equal(2, cacheValue2);

            var key3 = "TestCacheShell3";
            var cacheValue3 = this.cache.CacheShell<long>(key3, TimeSpan.FromSeconds(1), () => 3);
            Assert.Equal(3, cacheValue3);
        }

        /// <summary>
        /// 测试CacheShellAsync方法
        /// </summary>
        [Fact]
        public async Task TestCacheShellAsync()
        {
            //已有缓存的情况
            var key1 = "TestCacheShellAsync1";
            await this.cache.SetAsync(key1, "1", TimeSpan.FromMinutes(1));
            var cacheValue1 = await this.cache.CacheShellAsync<string>(key1, TimeSpan.FromSeconds(1), async () => await Task.FromResult("2"));
            Assert.Equal("1", cacheValue1);

            //未有缓存的情况
            var key2 = "TestCacheShellAsync2";
            var cacheValue2 = await this.cache.CacheShellAsync<int?>(key2, TimeSpan.FromSeconds(1), async () => await Task.FromResult(2));
            Assert.Equal(2, cacheValue2);

            var key3 = "TestCacheShellAsync3";
            var cacheValue3 = await this.cache.CacheShellAsync<long>(key3, TimeSpan.FromSeconds(1), async () => await Task.FromResult(3));
            Assert.Equal(3, cacheValue3);
        }

        /// <summary>
        /// 测试HCacheShell方法
        /// </summary>
        [Fact]
        public void TestHCacheShell()
        {
            var key = "TestHCacheShell";

            //已有缓存的情况
            this.cache.HSet(key, "field1", "1");
            var cacheValue1 = this.cache.HCacheShell<string>(key, TimeSpan.FromMinutes(1), "field1", () => "2");
            Assert.Equal("1", cacheValue1);

            //未有缓存的情况
            var cacheValue2 = this.cache.HCacheShell<int?>(key, TimeSpan.FromMinutes(1), "field2", () => 2);
            Assert.Equal(2, cacheValue2);

            var cacheValue3 = this.cache.HCacheShell<long>(key, TimeSpan.FromMinutes(1), "field3", () => 3);
            Assert.Equal(3, cacheValue3);

            Assert.Equal(3, this.cache.HLen(key));

            this.cache.Expire(key, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 测试HCacheShellAsync方法
        /// </summary>
        [Fact]
        public async Task TestHCacheShellAsync()
        {
            var key = "TestHCacheShellAsync";

            //已有缓存的情况
            await this.cache.HSetAsync(key, "field1", "1");
            var cacheValue1 = await this.cache.HCacheShellAsync<string>(key, TimeSpan.FromMinutes(1), "field1", async () => await Task.FromResult("2"));
            Assert.Equal("1", cacheValue1);

            //未有缓存的情况
            var cacheValue2 = await this.cache.HCacheShellAsync<int?>(key, TimeSpan.FromMinutes(1), "field2", async () => await Task.FromResult(2));
            Assert.Equal(2, cacheValue2);

            var cacheValue3 = await this.cache.HCacheShellAsync<long>(key, TimeSpan.FromMinutes(1), "field3", async () => await Task.FromResult(3));
            Assert.Equal(3, cacheValue3);

            Assert.Equal(3, this.cache.HLen(key));

            this.cache.Expire(key, TimeSpan.FromMinutes(1));
        }

        #region 内部测试用类

        /// <summary>
        /// 缓存对象
        /// </summary>
        public class ClassOjbect
        {
            public string Text { get; set; }
            public int Age { get; set; }
            public EnmGender GenderCode { get; set; }
            public ClassOjbect2 C2 { get; set; }
            public StructObject2 S2;
        }

        /// <summary>
        /// 缓存对象
        /// </summary>
        public class ClassOjbect2
        {
            public string Text2 { get; set; }
            public int Age2 { get; set; }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public enum EnmGender
        {
            Male = 1,
            Female = 2
        }

        /// <summary>
        /// 缓存对象
        /// </summary>
        public struct StructObject2
        {
            public string Text2 { get; set; }
            public int Age2 { get; set; }
        }

        #endregion
    }
}
