using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;

namespace tdb.account.domain.Authority.Aggregate
{
    /// <summary>
    /// 权限聚合
    /// </summary>
    public class AuthorityAgg : TdbAggregateRoot<int>
    {
        #region 值

        /// <summary>
        /// 权限名称
        /// </summary>           
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>           
        public string Remark { get; set; }

        #endregion
    }
}
