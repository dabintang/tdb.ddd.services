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
    /// 通过邀请码加入人际圈 请求参数
    /// </summary>
    public class JoinByInvitationCodeReq
    {
        /// <summary>
        /// [必须]邀请码
        /// </summary>
        [TdbRequired("邀请码")]
        [TdbStringLength("邀请码", 255)]
        public string Code { get; set; } = "";
    }

    /// <summary>
    /// 通过邀请码加入人际圈成功返回结果
    /// </summary>
    public class JoinByInvitationCodeRes
    {
        /// <summary>
        /// [必须]人际圈ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long CircleID { get; set; }

        /// <summary>
        /// 人际圈名称
        /// </summary>
        public string CircleName { get; set; } = "";
    }
}
