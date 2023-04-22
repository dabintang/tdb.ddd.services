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
    /// 读取加入人际圈的邀请码信息 请求参数
    /// </summary>
    public class ReadInvitationCodeReq
    {
        /// <summary>
        /// [必须]邀请码
        /// </summary>
        [TdbRequired("邀请码")]
        public string Code { get; set; } = "";
    }

    /// <summary>
    /// 读取加入人际圈的邀请码信息 结果
    /// </summary>
    public class ReadInvitationCodeRes
    {
        /// <summary>
        /// 人际圈名称
        /// </summary>
        public string CircleName { get; set; } = "";

        /// <summary>
        /// 邀请人姓名
        /// </summary>
        public string InviterName { get; set; } = "";

        /// <summary>
        /// 过期时间点
        /// </summary>
        public DateTime ExpireAt { get; set; }
    }
}
