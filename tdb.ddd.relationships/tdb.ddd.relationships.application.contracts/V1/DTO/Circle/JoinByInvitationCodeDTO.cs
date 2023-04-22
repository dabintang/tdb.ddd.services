using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Circle
{
    /// <summary>
    /// 通过邀请码加入人际圈 请求参数
    /// </summary>
    public class JoinByInvitationCodeReq
    {
        /// <summary>
        /// [必须]邀请码
        /// </summary>
        [TdbRequired("邀请码")]
        public string Code { get; set; } = "";
    }
}
