using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Personnel
{
    /// <summary>
    /// 根据用户ID获取人员信息 请求条件
    /// </summary>
    public class GetPersonnelByUserIDReq
    {
        /// <summary>
        /// [必须]用户ID
        /// </summary>
        [TdbHashIDModelBinder]
        [TdbRequired("用户ID")]
        public long UserID { get; set; }
    }


    /// <summary>
    /// 根据用户ID获取人员信息 结果
    /// </summary>
    public class GetPersonnelByUserIDRes : PersonnelInfo
    {
    }
}
