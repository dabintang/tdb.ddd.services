using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;

namespace tdb.account.domain.Role.Aggregate
{
    /// <summary>
    /// 角色聚合
    /// </summary>
    public class RoleAgg : TdbAggregateRoot<int>
    {
        #region 值

        /// <summary>
        /// 角色名称
        /// </summary>           
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>           
        public string Remark { get; set; }

        /// <summary>
        /// 角色拥有的权限ID
        /// </summary>
        public RoleAuthorityIDLazyLoad LstAuthorityID { get; private set; }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public RoleAgg()
        {
            this.LstAuthorityID = new RoleAuthorityIDLazyLoad(this);
        }
    }
}
