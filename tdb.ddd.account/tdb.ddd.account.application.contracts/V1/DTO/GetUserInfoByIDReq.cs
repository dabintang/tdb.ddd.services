using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.ddd.account.application.contracts.V1.DTO
{
    /// <summary>
    /// 根据用户ID获取用户信息 请求参数
    /// </summary>
    public class GetUserInfoByIDReq
    {
        /// <summary>
        /// [必须]用户ID
        /// </summary>
        [TdbHashIDModelBinder]
        [TdbRequired("用户编号")]
        public long UserID { get; set; }
    }
}
