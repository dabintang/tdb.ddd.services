using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Circle
{
    /// <summary>
    /// 移出成员 请求参数
    /// </summary>
    public class RemoveMemberReq
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
    }
}
