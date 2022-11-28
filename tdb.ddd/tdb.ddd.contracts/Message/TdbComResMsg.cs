using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 通用返回消息（范围：0-999）
    /// </summary>
    public class TdbComResMsg
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static readonly TdbResMsgInfo Success = new(0, "成功");

        /// <summary>
        /// 失败
        /// </summary>
        public static readonly TdbResMsgInfo Fail = new(1, "失败");

        /// <summary>
        /// 非法参数
        /// </summary>
        public static readonly TdbResMsgInfo InvalidParam = new(2, "非法参数");

        /// <summary>
        /// 权限不足
        /// </summary>
        public static readonly TdbResMsgInfo InsufficientPermissions = new (3, "权限不足");

        /// <summary>
        /// 操作用户不存在
        /// </summary>
        public static readonly TdbResMsgInfo UserNotExist = new(4, "操作用户不存在");
    }
}
