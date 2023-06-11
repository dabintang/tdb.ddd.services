using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Personnel
{
    /// <summary>
    /// 获取人员信息 请求条件
    /// </summary>
    public class GetPersonnelReq
    {
        /// <summary>
        /// [必须]人员ID
        /// </summary>
        [TdbHashIDModelBinder]
        [TdbRequired("人员ID")]
        public long ID { get; set; }
    }

    /// <summary>
    /// 获取人员信息 结果
    /// </summary>
    public class GetPersonnelRes : PersonnelInfo
    {
    }
}
