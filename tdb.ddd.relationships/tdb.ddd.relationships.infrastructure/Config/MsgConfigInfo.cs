using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.infrastructure.Config
{
    /// <summary>
    /// 回报消息配置
    /// </summary>
    public class MsgConfigInfo
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// 人际圈不存在
        /// </summary>
        public TdbResMsgInfo CircleNotExist { get; set; }

        /// <summary>
        /// 人员不存在
        /// </summary>
        public TdbResMsgInfo PersonnelNotExist { get; set; }

        /// <summary>
        /// 圈内无该成员
        /// </summary>
        public TdbResMsgInfo MemberNotExist { get; set; }

        /// <summary>
        /// 已达到人数上限
        /// </summary>
        public TdbResMsgInfo ReachedMemberLimit { get; set; }

        /// <summary>
        /// 无效邀请码
        /// </summary>
        public TdbResMsgInfo InvalidInvitationCode { get; set; }

        /// <summary>
        /// 邀请码已过期
        /// </summary>
        public TdbResMsgInfo ExpiredInvitationCode { get; set; }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}
