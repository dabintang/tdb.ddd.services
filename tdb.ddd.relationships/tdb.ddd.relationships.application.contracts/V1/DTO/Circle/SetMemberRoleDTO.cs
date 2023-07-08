using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.relationships.domain.contracts.Enum;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Circle
{
    /// <summary>
    /// 设置成员角色 请求参数
    /// </summary>
    public class SetMemberRoleReq
    {
        /// <summary>
        /// [必须]人际圈ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("人际圈ID")]
        public long CircleID { get; set; }

        /// <summary>
        /// [必须]人员ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("人员ID")]
        public long PersonnelID { get; set; }

        /// <summary>
        /// [必须]角色编码
        /// </summary>
        [TdbRequired("角色编码")]
        [TdbEnumDataType(typeof(EnmRole), "角色编码不正确")]
        public EnmRole RoleCode { get; set; }
    }
}
