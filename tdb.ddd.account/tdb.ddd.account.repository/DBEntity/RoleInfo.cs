using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.account.repository.DBEntity
{
    /// <summary>
    /// 角色信息表
    /// </summary>
    [SugarTable("role_info")]
    public class RoleInfo
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = 255)]
        public string Remark { get; set; } = string.Empty;
    }
}
