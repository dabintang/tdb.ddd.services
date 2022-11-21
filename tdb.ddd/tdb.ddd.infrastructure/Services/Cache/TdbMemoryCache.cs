using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 内存缓存
    /// （注：获取到的缓存对象为副本！如需修改缓存内容，请在获取缓存对象并修改后重新set到缓存中！）
    /// </summary>
    public class TdbMemoryCache : ITdbCache
    {
        /// <summary>
        /// 内存缓存
        /// </summary>
        private readonly IMemoryCache cache;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="option">内存缓存配置</param>
        public TdbMemoryCache(MemoryCacheOptions? option = null)
        {
            option ??= new MemoryCacheOptions();

            this.cache = new MemoryCache(option);
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
            var value = this.GetInner<T>(key);
            //深复制
            var copyValue = value.DeepClone();
            return copyValue;
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
            this.cache.Set(key, value, expire);
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
            this.Set(key, value, expire);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 用于在 key 存在时删除 key
        /// </summary>
        /// <param name="keys"></param>
        public void Del(params string[] keys)
        {
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    this.cache.Remove(key);
                }
            }
        }

        /// <summary>
        /// 用于在 key 存在时删除 key
        /// </summary>
        /// <param name="keys"></param>
        public async Task DelAsync(params string[] keys)
        {
            this.Del(keys);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key"></param>
        public bool Exists(string key)
        {
            var keys = this.Keys("*");
            return keys.Contains(key);
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        public void Expire(string key, TimeSpan expire)
        {
            if (this.cache.TryGetValue(key, out object? objVal))
            {
                this.Set(key, objVal, expire);
            }
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        public async Task ExpireAsync(string key, TimeSpan expire)
        {
            this.Expire(key, expire);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 为给定 key 设置过期时间点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt">过期时间点</param>
        public void ExpireAt(string key, DateTime expireAt)
        {
            var expire = expireAt - DateTime.Now;
            if (expire.TotalSeconds > 0)
            {
                this.Expire(key, expire);
            }
            else
            {
                this.Del(key);
            }
        }

        /// <summary>
        /// 为给定 key 设置过期时间点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt">过期时间点</param>
        public async Task ExpireAtAsync(string key, DateTime expireAt)
        {
            this.ExpireAt(key, expireAt);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 查找所有分区节点中符合给定模式(pattern)的 key
        /// </summary>
        /// <param name="pattern">如：runoob*</param>
        /// <returns></returns>
        public string[] Keys(string pattern)
        {
            var keys = new List<string>();

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var coherentState = cache.GetType().GetField("_coherentState", flags)?.GetValue(cache);
            if (coherentState == null)
            {
                return keys.ToArray();
            }

            //((MemoryCache.CoherentState)coherentState).EntriesCollection

            var entries = coherentState.GetType().GetField("_entries", flags)?.GetValue(coherentState);
            if (entries is not IDictionary cacheItems)
            {
                return keys.ToArray();
            }

            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                //判断缓存是否已过期
                if ((cacheItem.Value is ICacheEntry cacheEntry) && 
                    (cacheEntry.AbsoluteExpiration.HasValue && cacheEntry.AbsoluteExpiration.Value.LocalDateTime < DateTime.Now))
                {
                    continue;
                }

                var key = cacheItem.Key.ToString() ?? "";
                if (pattern == "*" || string.IsNullOrEmpty(pattern))
                {
                    keys.Add(key);
                }
                else if (pattern.EndsWith("*") && key.StartsWith(pattern[..^1]))
                {
                    keys.Add(key);
                }
                else if (pattern.StartsWith("*") && key.EndsWith(pattern[1..]))
                {
                    keys.Add(key);
                }
                else if (pattern.StartsWith("*") && pattern.EndsWith("*") && key.Contains(pattern[1..^1]))
                {
                    keys.Add(key);
                }
                else if (key == pattern)
                {
                    keys.Add(key);
                }
            }
            return keys.ToArray();
        }

        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        public T? HGet<T>(string key, string field)
        {
            var value = this.HGetInner<T>(key, field);
            //深复制
            var copyValue = value.DeepClone();
            return copyValue;
        }

        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, T?>? HGetAll<T>(string key)
        {
            var value = this.HGetAllInner(key);
            if (value == null)
            {
                return null;
            }

            //深复制
            var resultValue = new Dictionary<string, T?>();
            foreach (var item in value)
            {
                resultValue[item.Key] = ((T?)item.Value).DeepClone();
            }

            return resultValue;
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段名</param>
        /// <param name="value">值</param>
        public void HSet<T>(string key, string field, T? value)
        {
            this.HSet(key, TimeSpan.FromDays(10000), field, value);
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
            var dic = this.HGetAllInner(key);
            if (dic == null)
            {
                dic = new Dictionary<string, object?>();
                this.Set(key, dic, expire);
            }

            dic[field] = value;
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段名</param>
        /// <param name="value">值</param>
        public async Task HSetAsync<T>(string key, string field, T? value)
        {
            this.HSet(key, field, value);
            await Task.CompletedTask;
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
            this.HSet(key, expire, field, value);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        public void HMSet<T>(string key, params (string, T?)[] keyValues)
        {
            this.HMSet(key, TimeSpan.FromDays(10000), keyValues);
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
            if (keyValues == null)
            {
                return;
            }

            var dic = this.HGetAllInner(key);
            if (dic == null)
            {
                dic = new Dictionary<string, object?>();
                this.Set(key, dic, expire);
            }

            for (int i = 0; i < keyValues.Length; i++)
            {
                var field = keyValues[i].Item1;
                var value = keyValues[i].Item2;

                dic[field] = value;
            }
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        public async Task HMSetAsync<T>(string key, params (string, T?)[] keyValues)
        {
            this.HMSet(key, keyValues);
            await Task.CompletedTask;
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
            this.HMSet(key, expire, keyValues);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields">字段名</param>
        public void HDel(string key, params string[] fields)
        {
            if (fields == null)
            {
                return;
            }

            var dic = this.HGetAllInner(key);
            if (dic == null)
            {
                return;
            }

            foreach (var field in fields)
            {
                dic.Remove(field);
            }
        }

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields">字段名</param>
        public async Task HDelAsync(string key, params string[] fields)
        {
            this.HDel(key, fields);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 查看哈希表 key 中，指定的字段是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        public bool HExists(string key, string field)
        {
            var dic = this.HGetAllInner(key);
            if (dic == null)
            {
                return false;
            }

            return dic.ContainsKey(field);
        }

        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] HKeys(string key)
        {
            var dic = this.HGetAllInner(key);
            if (dic == null)
            {
                return Array.Empty<string>();
            }

            return dic.Keys.ToArray();
        }

        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long HLen(string key)
        {
            var dic = this.HGetAllInner(key);
            if (dic == null)
            {
                return 0;
            }

            return dic.Count;
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
            var value = this.CacheShellInner<T>(key, expire, getData);
            //深复制
            var copyValue = value.DeepClone();
            return copyValue;
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
            var value = await this.CacheShellInnerAsync<T>(key, expire, getDataAsync);
            //深复制
            var copyValue = value.DeepClone();
            return copyValue;
        }

        /// <summary>
        /// 缓存壳(哈希表)
        /// </summary>
        /// <typeparam name="T">必须是可空类型</typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间（每次缓存会重新设置过期时间）</param>
        /// <param name="field">字段名</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        public T? HCacheShell<T>(string key, TimeSpan expire, string field, Func<T?> getData)
        {
            var value = this.HCacheShellInner<T>(key, expire, field, getData);
            //深复制
            var copyValue = value.DeepClone();
            return copyValue;
        }

        /// <summary>
        /// 缓存壳(哈希表)
        /// </summary>
        /// <typeparam name="T">必须是可空类型</typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间（每次缓存会重新设置过期时间）</param>
        /// <param name="field">字段名</param>
        /// <param name="getDataAsync">获取源数据的函数</param>
        /// <returns></returns>
        public async Task<T?> HCacheShellAsync<T>(string key, TimeSpan expire, string field, Func<Task<T?>> getDataAsync)
        {
            var value = await this.HCacheShellInnerAsync<T>(key, expire, field, getDataAsync);
            //深复制
            var copyValue = value.DeepClone();
            return copyValue;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private T? GetInner<T>(string key)
        {
            return this.cache.Get<T>(key);
        }

        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        private T? HGetInner<T>(string key, string field)
        {
            var dic = this.HGetAllInner(key);
            if (dic == null)
            {
                return default;
            }

            if (dic.TryGetValue(field, out object? value))
            {
                return (T?)value;
            }

            return default;
        }

        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private Dictionary<string, object?>? HGetAllInner(string key)
        {
            if (this.cache.TryGetValue(key, out Dictionary<string, object?>? dicVal) && dicVal != null)
            {
                return dicVal;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 缓存壳
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        private T? CacheShellInner<T>(string key, TimeSpan expire, Func<T?> getData)
        {
            //先从缓存获取看是否已有缓存
            var value = this.GetInner<T>(key);
            if (value != null && (!value.Equals(default(T)) || this.Exists(key)))
            {
                return value;
            }

            //上锁，避免大量并发进入
            using (var lockRet = TdbLocalLock.Lock(key))
            {
                //再次尝试从缓存获取值
                var valAgain = this.GetInner<T>(key);
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
        private async Task<T?> CacheShellInnerAsync<T>(string key, TimeSpan expire, Func<Task<T?>> getDataAsync)
        {
            //先从缓存获取看是否已有缓存
            var value = this.GetInner<T>(key);
            if (value != null && (!value.Equals(default(T)) || this.Exists(key)))
            {
                return value;
            }

            //上锁，避免大量并发进入
            using (var lockRet = TdbLocalLock.Lock(key))
            {
                //再次尝试从缓存获取值
                var valAgain = this.GetInner<T>(key);
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
        /// <param name="field">字段名</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        private T? HCacheShellInner<T>(string key, TimeSpan expire, string field, Func<T?> getData)
        {
            //先从缓存获取看是否已有缓存
            var value = this.HGetInner<T>(key, field);
            if (value != null && (!value.Equals(default(T)) || this.HExists(key, field)))
            {
                return value;
            }

            //上锁，避免大量并发进入
            using (var lockRet = TdbLocalLock.Lock(key))
            {
                //再次尝试从缓存获取值
                var valAgain = this.HGetInner<T>(key, field);
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
        /// <param name="field">字段名</param>
        /// <param name="getDataAsync">获取源数据的函数</param>
        /// <returns></returns>
        private async Task<T?> HCacheShellInnerAsync<T>(string key, TimeSpan expire, string field, Func<Task<T?>> getDataAsync)
        {
            //先从缓存获取看是否已有缓存
            var value = this.HGetInner<T>(key, field);
            if (value != null && (!value.Equals(default(T)) || this.HExists(key, field)))
            {
                return value;
            }

            //上锁，避免大量并发进入
            using (var lockRet = TdbLocalLock.Lock(key))
            {
                //再次尝试从缓存获取值
                var valAgain = this.HGetInner<T>(key, field);
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
