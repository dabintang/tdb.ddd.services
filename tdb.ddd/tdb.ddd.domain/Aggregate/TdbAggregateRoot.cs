using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.domain
{
    /// <summary>
    /// 聚合根
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class TdbAggregateRoot<TKey> : TdbEntity<TKey>
    {
    }
}
