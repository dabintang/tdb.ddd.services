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
    /// 生成加入人际圈的邀请码 请求参数
    /// </summary>
    public class CreateInvitationCodeReq
    {
        /// <summary>
        /// [必须]人际圈ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("人际圈ID")]
        public long CircleID { get; set; }

        /// <summary>
        /// [必须]有效时间（分钟）
        /// </summary>
        [TdbNumRange("有效时间", ErrorMessage = "有效时间超出范围[1-14400]", MinValue = 1, MaxValue = 14400)]
        public int EffectiveMinutes { get; set; }
    }

    /// <summary>
    /// 生成加入人际圈的邀请码 结果
    /// </summary>
    public class CreateInvitationCodeRes
    {
        /// <summary>
        /// 邀请码
        /// </summary>
        public string Code { get; set; } = "";

        /// <summary>
        /// 过期时间点
        /// </summary>
        public DateTime ExpireAt { get; set; }
    }
}
