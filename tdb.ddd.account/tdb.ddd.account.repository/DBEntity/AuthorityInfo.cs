using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.account.repository.DBEntity
{
    /// <summary>
    /// 权限信息表
    /// </summary>
    [SugarTable("authority_info")]
    public class AuthorityInfo
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Name { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = 255)]
        public string Remark { get; set; } = "";
    }
}
