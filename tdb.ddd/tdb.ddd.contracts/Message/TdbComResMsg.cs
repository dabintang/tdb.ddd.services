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
    }
}
