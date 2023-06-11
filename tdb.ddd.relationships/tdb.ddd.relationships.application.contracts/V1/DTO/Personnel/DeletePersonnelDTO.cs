using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Personnel
{
    /// <summary>
    /// 删除人员信息 条件
    /// </summary>
    public class DeletePersonnelReq
    {
        /// <summary>
        /// [必须]人员ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("人员ID")]
        public long ID { get; set; }
    }
}
