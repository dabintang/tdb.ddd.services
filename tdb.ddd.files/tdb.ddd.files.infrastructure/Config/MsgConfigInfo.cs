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
        /// <summary>
        /// 文件不存在
        /// </summary>
        public TdbResMsgInfo FileNotExist { get; set; }

        /// <summary>
        /// 不是图片
        /// </summary>
        public TdbResMsgInfo NotImage { get; set; }
    }
}
