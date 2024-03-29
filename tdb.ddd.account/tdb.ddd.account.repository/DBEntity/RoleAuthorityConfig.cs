﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.account.repository.DBEntity
{
    /// <summary>
    /// 角色权限配置表
    /// </summary>
    [SugarTable("role_authority_config")]
    public class RoleAuthorityConfig
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long RoleID { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long AuthorityID { get; set; }
    }
}
