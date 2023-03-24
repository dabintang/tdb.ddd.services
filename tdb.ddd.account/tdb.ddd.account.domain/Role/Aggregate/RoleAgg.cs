using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.BusMediatR;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.account.domain.contracts.User;
using tdb.ddd.account.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure.Services;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Role.Aggregate
{
    /// <summary>
    /// 角色聚合
    /// </summary>
    public class RoleAgg : TdbAggregateRoot<long>
    {
        #region 值

        /// <summary>
        /// 角色名称
        /// </summary>           
        public string Name { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>           
        public string Remark { get; set; } = "";

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

        #region 行为

        /// <summary>
        /// 赋予权限（全量赋值）
        /// </summary>
        /// <param name="lstAuthorityID">权限ID</param>
        public TdbRes<bool> SetLstAuthorityID(List<long> lstAuthorityID)
        {
            lstAuthorityID ??= new List<long>();
            return this.LstAuthorityID.SetValue(lstAuthorityID);
        }

        #endregion
    }
}
