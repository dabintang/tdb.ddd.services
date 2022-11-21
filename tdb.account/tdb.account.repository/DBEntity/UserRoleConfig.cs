using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.account.repository.DBEntity
{
    /// <summary>
    /// 用户角色配置表
    /// </summary>
    [SugarTable("user_role_config")]
    public class UserRoleConfig
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long UserID { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int RoleID { get; set; }
    }
}
