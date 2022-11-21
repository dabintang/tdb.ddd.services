using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ITdbCache
    {
        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T? Get<T>(string key);

        /// <summary>
        /// 设置指定 key 的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        void Set<T>(string key, T? value, TimeSpan expire);

        /// <summary>
        /// 设置指定 key 的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        Task SetAsync<T>(string key, T? value, TimeSpan expire);

        /// <summary>
        /// 用于在 key 存在时删除 key
        /// </summary>
        /// <param name="keys"></param>
        void Del(params string[] keys);

        /// <summary>
        /// 用于在 key 存在时删除 key
        /// </summary>
        /// <param name="keys"></param>
        Task DelAsync(params string[] keys);

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key"></param>
        bool Exists(string key);

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        void Expire(string key, TimeSpan expire);

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        Task ExpireAsync(string key, TimeSpan expire);

        /// <summary>
        /// 为给定 key 设置过期时间点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt">过期时间点</param>
        void ExpireAt(string key, DateTime expireAt);

        /// <summary>
        /// 为给定 key 设置过期时间点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt">过期时间点</param>
        Task ExpireAtAsync(string key, DateTime expireAt);

        /// <summary>
        /// 查找所有分区节点中符合给定模式(pattern)的 key
        /// </summary>
        /// <param name="pattern">如：runoob*</param>
        /// <returns></returns>
        string[] Keys(string pattern);

        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        T? HGet<T>(string key, string field);

        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Dictionary<string, T?>? HGetAll<T>(string key);

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段名</param>
        /// <param name="value">值</param>
        void HSet<T>(string key, string field, T? value);

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间</param>
        /// <param name="field">字段名</param>
        /// <param name="value">值</param>
        void HSet<T>(string key, TimeSpan expire, string field, T? value);

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段名</param>
        /// <param name="value">值</param>
        Task HSetAsync<T>(string key, string field, T? value);

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间</param>
        /// <param name="field">字段名</param>
        /// <param name="value">值</param>
        Task HSetAsync<T>(string key, TimeSpan expire, string field, T? value);

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        void HMSet<T>(string key, params (string, T?)[] keyValues);

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间</param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        void HMSet<T>(string key, TimeSpan expire, params (string, T?)[] keyValues);

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        Task HMSetAsync<T>(string key, params (string, T?)[] keyValues);

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间</param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        Task HMSetAsync<T>(string key, TimeSpan expire, params (string, T?)[] keyValues);

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields">字段名</param>
        void HDel(string key, params string[] fields);

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields">字段名</param>
        Task HDelAsync(string key, params string[] fields);

        /// <summary>
        /// 查看哈希表 key 中，指定的字段是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        bool HExists(string key, string field);

        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        string[] HKeys(string keys);

        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long HLen(string key);

        /// <summary>
        /// 缓存壳
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        T? CacheShell<T>(string key, TimeSpan expire, Func<T?> getData);

        /// <summary>
        /// 缓存壳
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <param name="getDataAsync">获取源数据的函数</param>
        /// <returns></returns>
        Task<T?> CacheShellAsync<T>(string key, TimeSpan expire, Func<Task<T?>> getDataAsync);

        /// <summary>
        /// 缓存壳(哈希表)
        /// </summary>
        /// <typeparam name="T">必须是可空类型</typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间（每次缓存会重新设置过期时间）</param>
        /// <param name="field">字段名</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        T? HCacheShell<T>(string key, TimeSpan expire, string field, Func<T?> getData);

        /// <summary>
        /// 缓存壳(哈希表)
        /// </summary>
        /// <typeparam name="T">必须是可空类型</typeparam>
        /// <param name="key"></param>
        /// <param name="expire">key过期时间（每次缓存会重新设置过期时间）</param>
        /// <param name="field">字段名</param>
        /// <param name="getDataAsync">获取源数据的函数</param>
        /// <returns></returns>
        Task<T?> HCacheShellAsync<T>(string key, TimeSpan expire, string field, Func<Task<T?>> getDataAsync);
    }
}
