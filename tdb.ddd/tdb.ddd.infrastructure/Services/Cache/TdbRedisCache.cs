using CSRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// redis缓存
    /// </summary>
    public class TdbRedisCache : ITdbCache
    {
        /// <summary>
        /// csredis
        /// </summary>
        protected CSRedisClient rds;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionStrings">连接字符串集合</param>
        public TdbRedisCache(params string[] connectionStrings)
        {
            this.rds = new CSRedis.CSRedisClient((p => { return null; }), connectionStrings);
            //RedisHelper.Initialization(this.rds);
        }

        #region 实现接口

        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? Get<T>(string key)
        {
            return this.rds.Get<T>(key);
        }

        /// <summary>
        /// 设置指定 key 的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        public void Set<T>(string key, T? value, TimeSpan expire)
        {
            this.rds.Set(key, value, expire);
        }

        /// <summary>
        /// 设置指定 key 的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        public async Task SetAsync<T>(string key, T? value, TimeSpan expire)
        {
            await this.rds.SetAsync(key, value, expire);
        }

        /// <summary>
        /// 用于在 key 存在时删除 key
        /// </summary>
        /// <param name="keys"></param>
        public void Del(params string[] keys)
        {
            this.rds.Del(keys);
        }

        /// <summary>
        /// 用于在 key 存在时删除 key
        /// </summary>
        /// <param name="keys"></param>
        public async Task DelAsync(params string[] keys)
        {
            await this.rds.DelAsync(keys);
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key"></param>
        public bool Exists(string key)
        {
            return this.rds.Exists(key);
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        public void Expire(string key, TimeSpan expire)
        {
            this.rds.Expire(key, expire);
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        public async Task ExpireAsync(string key, TimeSpan expire)
        {
            await this.rds.ExpireAsync(key, expire);
        }

        /// <summary>
        /// 为给定 key 设置过期时间点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt">过期时间点</param>
        public void ExpireAt(string key, DateTime expireAt)
        {
            this.rds.ExpireAt(key, expireAt);
        }

        /// <summary>
        /// 为给定 key 设置过期时间点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt">过期时间点</param>
        public async Task ExpireAtAsync(string key, DateTime expireAt)
        {
            await this.rds.ExpireAtAsync(key, expireAt);
        }

        /// <summary>
        /// 查找所有分区节点中符合给定模式(pattern)的 key
        /// </summary>
        /// <param name="pattern">如：runoob*</param>
        /// <returns></returns>
        public string[] Keys(string pattern)
        {
            return this.rds.Keys(pattern);
        }

        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public T HGet<T>(string key, string field)
        {
            return this.rds.HGet<T>(key, field);
        }

        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, T?>? HGetAll<T>(string key)
        {
#pragma warning disable CS8619 // 值中的引用类型的为 Null 性与目标类型不匹配。
            return this.rds.HGetAll<T>(key);
#pragma warning restore CS8619 // 值中的引用类型的为 Null 性与目标类型不匹配。
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        public void HSet<T>(string key, string field, T? value)
        {
            this.rds.HSet(key, field, value);
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间</param>
        /// <param name="field">字段名</param>
        /// <param name="value">值</param>
        public void HSet<T>(string key, TimeSpan expire, string field, T? value)
        {
            this.HSet(key, field, value);
            this.Expire(key, expire);
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        public async Task HSetAsync<T>(string key, string field, T? value)
        {
            await this.rds.HSetAsync(key, field, value);
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间</param>
        /// <param name="field">字段名</param>
        /// <param name="value">值</param>
        public async Task HSetAsync<T>(string key, TimeSpan expire, string field, T? value)
        {
            await this.HSetAsync(key, field, value);
            await this.ExpireAsync(key, expire);
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// 
        /// <param name="key"></param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        public void HMSet<T>(string key, params (string, T?)[] keyValues)
        {
            var kvs = keyValues.SelectMany(m => new object?[] { m.Item1, m.Item2 }).ToArray();
            this.rds.HMSet(key, kvs);
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间</param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        public void HMSet<T>(string key, TimeSpan expire, params (string, T?)[] keyValues)
        {
            this.HMSet(key, keyValues);
            this.Expire(key, expire);
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        public async Task HMSetAsync<T>(string key, params (string, T?)[] keyValues)
        {
            var kvs = keyValues.SelectMany(m => new object?[] { m.Item1, m.Item2 }).ToArray();
            await this.rds.HMSetAsync(key, kvs);
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间</param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        public async Task HMSetAsync<T>(string key, TimeSpan expire, params (string, T?)[] keyValues)
        {
            await this.HMSetAsync<T>(key, keyValues);
            await this.ExpireAsync(key, expire);
        }

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        public void HDel(string key, params string[] fields)
        {
            this.rds.HDel(key, fields);
        }

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        public async Task HDelAsync(string key, params string[] fields)
        {
            await this.rds.HDelAsync(key, fields);
        }

        /// <summary>
        /// 查看哈希表 key 中，指定的字段是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool HExists(string key, string field)
        {
            return this.rds.HExists(key, field);
        }

        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] HKeys(string key)
        {
            return this.rds.HKeys(key);
        }

        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long HLen(string key)
        {
            return this.rds.HLen(key);
        }

        /// <summary>
        /// 缓存壳
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        public T? CacheShell<T>(string key, TimeSpan expire, Func<T?> getData)
        {
            //先从缓存获取看是否已有缓存
            var value = this.Get<T>(key);
            if (value != null && (!value.Equals(default(T)) || this.Exists(key)))
            {
                return value;
            }

            //上锁
            using (var lockRet = TdbLocalLock.Lock(key))
            {
                //再次尝试从缓存获取值
                var valAgain = this.Get<T>(key);
                if (valAgain != null && (!valAgain.Equals(default(T)) || this.Exists(key)))
                {
                    return valAgain;
                }

                //获取源数据
                value = getData();
            }

            //缓存
            if (value != null)
            {
                this.Set(key, value, expire);
            }

            return value;
        }

        /// <summary>
        /// 缓存壳
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <param name="getDataAsync">获取源数据的函数</param>
        /// <returns></returns>
        public async Task<T?> CacheShellAsync<T>(string key, TimeSpan expire, Func<Task<T?>> getDataAsync)
        {
            //先从缓存获取看是否已有缓存
            var value = this.Get<T>(key);
            if (value != null && (!value.Equals(default(T)) || this.Exists(key)))
            {
                return value;
            }

            //上锁
            using (var lockRet = TdbLocalLock.Lock(key))
            {
                //再次尝试从缓存获取值
                var valAgain = this.Get<T>(key);
                if (valAgain != null && (!valAgain.Equals(default(T)) || this.Exists(key)))
                {
                    return valAgain;
                }

                //获取源数据
                value = await getDataAsync();
            }

            //缓存
            if (value != null)
            {
                await this.SetAsync(key, value, expire);
            }

            return value;
        }

        /// <summary>
        /// 缓存壳(哈希表)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间（每次缓存会重新设置过期时间）</param>
        /// <param name="field">字段</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        public T? HCacheShell<T>(string key, TimeSpan expire, string field, Func<T?> getData)
        {
            //先从缓存获取看是否已有缓存
            var value = this.HGet<T>(key, field);
            if (value != null && (!value.Equals(default(T)) || this.HExists(key, field)))
            {
                return value;
            }

            //上锁
            using (var lockRet = TdbLocalLock.Lock(key))
            {
                //再次尝试从缓存获取值
                var valAgain = this.HGet<T>(key, field);
                if (valAgain != null && (!valAgain.Equals(default(T)) || this.HExists(key, field)))
                {
                    return valAgain;
                }

                //获取源数据
                value = getData();

                //缓存
                if (value != null)
                {
                    this.HSet(key, field, value);
                    this.Expire(key, expire);
                }
            }

            return value;
        }

        /// <summary>
        /// 缓存壳(哈希表)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间（每次缓存会重新设置过期时间）</param>
        /// <param name="field">字段</param>
        /// <param name="getDataAsync">获取源数据的函数</param>
        /// <returns></returns>
        public async Task<T?> HCacheShellAsync<T>(string key, TimeSpan expire, string field, Func<Task<T?>> getDataAsync)
        {
            //先从缓存获取看是否已有缓存
            var value = this.HGet<T>(key, field);
            if (value != null && (!value.Equals(default(T)) || this.HExists(key, field)))
            {
                return value;
            }

            //上锁
            using (var lockRet = TdbLocalLock.Lock(key))
            {
                //再次尝试从缓存获取值
                var valAgain = this.HGet<T>(key, field);
                if (valAgain != null && (!valAgain.Equals(default(T)) || this.HExists(key, field)))
                {
                    return valAgain;
                }

                //获取源数据
                value = await getDataAsync();

                //缓存
                if (value != null)
                {
                    await this.HSetAsync(key, field, value);
                    await this.ExpireAsync(key, expire);
                }
            }

            return value;
        }

        #endregion
    }
}
