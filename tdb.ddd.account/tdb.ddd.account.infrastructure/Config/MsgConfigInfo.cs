using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.account.infrastructure.Config
{
    /// <summary>
    /// 回报消息配置
    /// </summary>
    public class MsgConfigInfo
    {
        /// <summary>
        /// 登录名或密码不对
        /// </summary>
        public TdbResMsgInfo IncorrectPassword { get; set; }

        /// <summary>
        /// 用户已被禁用
        /// </summary>
        public TdbResMsgInfo Disableduser { get; set; }

        /// <summary>
        /// 刷新令牌已失效
        /// </summary>
        public TdbResMsgInfo ExpiredRefreshToken { get; set; }

        /// <summary>
        /// 用户不存在
        /// </summary>
        public TdbResMsgInfo UserNotExist { get; set; }

        /// <summary>
        /// 登录名已存在
        /// </summary>
        public TdbResMsgInfo LoginNameExist { get; set; }

        /// <summary>
        /// 角色不存在
        /// </summary>
        public TdbResMsgInfo RoleNotExist { get; set; }

        /// <summary>
        /// 权限不存在
        /// </summary>
        public TdbResMsgInfo AuthorityNotExist { get; set; }

        /// <summary>
        /// 原密码不对
        /// </summary>
        public TdbResMsgInfo IncorrectOldPassword { get; set; }
    }
}
