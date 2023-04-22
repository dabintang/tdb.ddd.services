using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// 排序帮助类
    /// </summary>
    public static class TdbSortHelper
    {
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TSource">需排序的对象类型</typeparam>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="sortCode">排序方式（1：升序；2：降序）</param>
        /// <param name="keySelector">排序字段选择器</param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> Sort<TSource, TKey>(this IEnumerable<TSource> list, EnmTdbSort sortCode, Func<TSource, TKey> keySelector)
        {
            switch (sortCode)
            {
                case EnmTdbSort.Asc:
                    if (list is IOrderedEnumerable<TSource> listAsc)
                    {
                        return listAsc.ThenBy(m => keySelector(m));
                    }
                    else
                    {
                        return list.OrderBy(m => keySelector(m));
                    }
                case EnmTdbSort.Desc:
                    if (list is IOrderedEnumerable<TSource> listDesc)
                    {
                        return listDesc.ThenByDescending(m => keySelector(m));
                    }
                    else
                    {
                        return list.OrderByDescending(m => keySelector(m));
                    }
                default:
                    throw new TdbException($"不支持的排序方式{sortCode}");
            }
        }
    }
}
