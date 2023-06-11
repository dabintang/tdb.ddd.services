using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Role.Aggregate
{
    /// <summary>
    /// 角色聚合
    /// </summary>
    public class RoleAgg : TdbAggregateRoot<long>
    {
        #region 仓储

        private IRoleRepos? _roleRepos;
        /// <summary>
        /// 角色仓储
        /// </summary>
        private IRoleRepos RoleRepos
        {
            get
            {
                this._roleRepos ??= TdbIOC.GetService<IRoleRepos>();
                if (this._roleRepos is null)
                {
                    throw new TdbException("角色仓储接口未实现");
                }
                return this._roleRepos;
            }
        }

        #endregion

        #region 值

        /// <summary>
        /// 角色名称
        /// </summary>           
        public string Name { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>           
        public string Remark { get; set; } = "";

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public RoleAgg()
        {
        }

        #region 行为

        /// <summary>
        /// 获取角色拥有的权限ID
        /// </summary>
        /// <returns></returns>
        public async Task<List<long>> GetAuthorityIDsAsync()
        {
            return await this.RoleRepos.GetAuthorityIDsAsync(this.ID);
        }

        /// <summary>
        /// 赋予权限并保存（全量赋值）
        /// </summary>
        /// <param name="lstAuthorityID">权限ID</param>
        /// <returns></returns>
        public async Task SetAuthorityAndSaveAsync(List<long> lstAuthorityID)
        {
            await this.RoleRepos.SaveRoleAuthorityAsync(this.ID, lstAuthorityID);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await this.RoleRepos.SaveAsync(this);
        }

        #endregion
    }
}
