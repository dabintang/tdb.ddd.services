using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.BusMediatR;
using tdb.ddd.account.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.account.domain.Role.Aggregate
{
    /// <summary>
    /// 角色权限ID懒加载
    /// </summary>
    public class RoleAuthorityIDLazyLoad : TdbLazyLoadObject<List<long>>
    {
        #region 仓储

        private IRoleRepos _roleRepos;
        /// <summary>
        /// 角色仓储
        /// </summary>
        private IRoleRepos RoleRepos
        {
            get
            {
                this._roleRepos ??= TdbIOC.GetService<IRoleRepos>();
                return this._roleRepos;
            }
        }

        #endregion

        /// <summary>
        /// 角色聚合
        /// </summary>
        private readonly RoleAgg roleAgg;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="roleAgg">角色聚合</param>
        public RoleAuthorityIDLazyLoad(RoleAgg roleAgg)
        {
            this.roleAgg = roleAgg;
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        protected override List<long> Load()
        {
            if (this.roleAgg.ID <= 0)
            {
                throw new TdbException("角色权限ID懒加载异常，角色ID不正确");
            }

            return this.RoleRepos.GetAuthorityIDsAsync(this.roleAgg.ID).Result;
        }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="lstAuthorityID">权限ID</param>
        internal TdbRes<bool> SetValue(List<long> lstAuthorityID)
        {
            //判断权限ID是否存在
            foreach (var authorityID in lstAuthorityID)
            {
                if (TdbMediatR.Send(new IsAuthorityExistRequest() { AuthorityID = authorityID }).Result == false)
                {
                    return new TdbRes<bool>(AccountConfig.Msg.AuthorityNotExist.FromNewMsg($"权限不存在[{authorityID}]"), false);
                }
            }

            //赋值
            this.Value = lstAuthorityID;

            return TdbRes.Success(true);
        }
    }
}
