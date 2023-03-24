using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.files.infrastructure.Config
{
    /// <summary>
    /// 回报消息配置
    /// </summary>
    public class MsgConfigInfo
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// 文件不存在
        /// </summary>
        public TdbResMsgInfo FileNotExist { get; set; }

        /// <summary>
        /// 不是图片
        /// </summary>
        public TdbResMsgInfo NotImage { get; set; }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}
