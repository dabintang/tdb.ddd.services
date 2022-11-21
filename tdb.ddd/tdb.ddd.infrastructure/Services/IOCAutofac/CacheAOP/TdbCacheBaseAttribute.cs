using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 缓存特性基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TdbCacheBaseAttribute : Attribute
    {
    }
}
