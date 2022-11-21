using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// 本地锁，按字符串锁
    /// </summary>
    public class TdbLocalLock : IDisposable
    {
        /// <summary>
        /// 内存缓存
        /// </summary>
        private static MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

        #region 静态方法

        /// <summary>
        /// 对key上锁
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="maxWaitSeconds">最多等待时间（秒）</param>
        /// <returns></returns>
        public static TdbLocalLock Lock(string key, int maxWaitSeconds = 60)
        {
            DateTime startTime = DateTime.Now;
            string? lockVal = null;

            //保证lock时间比较短
            lock (memoryCache)
            {
                lockVal = memoryCache.Get<string>(key);
                if (lockVal == null)
                {
                    memoryCache.Set(key, "", TimeSpan.FromSeconds(MaxLockSecond));
                    return new TdbLocalLock(key, false);
                }
            }

            while (lockVal != null)
            {
                //超过等待
                if ((DateTime.Now - startTime).TotalSeconds > maxWaitSeconds)
                {
                    return new TdbLocalLock(key, true);
                }

                Thread.Sleep(30);

                //保证lock时间比较短
                lock (memoryCache)
                {
                    lockVal = memoryCache.Get<string>(key);
                    if (lockVal == null)
                    {
                        memoryCache.Set(key, "", TimeSpan.FromSeconds(MaxLockSecond));
                        return new TdbLocalLock(key, false);
                    }
                }
            }

            //代码应该不会进来到这里
            memoryCache.Set(key, "", TimeSpan.FromSeconds(MaxLockSecond));
            return new TdbLocalLock(key, false);
        }

        #endregion

        #region 变量/属性

        /// <summary>
        /// key
        /// </summary>
        private string Key { get; set; }

        /// <summary>
        /// 是否被他人锁着
        /// </summary>
        public bool IsLockedByOther { get; private set; }

        #endregion

        #region 常量

        /// <summary>
        /// 最大上锁时间（10000秒）
        /// </summary>
        private const int MaxLockSecond = 10000;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="isLockedByOther">是否被他人锁着</param>
        private TdbLocalLock(string key, bool isLockedByOther)
        {
            this.Key = key;
            this.IsLockedByOther = isLockedByOther;
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            //如果锁不在此实例，不做处理
            if (this.IsLockedByOther)
            {
                return;
            }

            //释放锁
            memoryCache.Remove(this.Key);
        }

        #endregion
    }
}
