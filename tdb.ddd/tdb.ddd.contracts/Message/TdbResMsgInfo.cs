using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 返回消息
    /// </summary>
    public class TdbResMsgInfo
    {
        /// <summary>
        /// 消息编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Msg { get; set; } = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        public TdbResMsgInfo()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">消息编码</param>
        /// <param name="msg">消息内容</param>
        public TdbResMsgInfo(int code, string msg)
        {
            this.Code = code;
            this.Msg = msg;
        }

        /// <summary>
        /// 用新消息内容生成消息体
        /// </summary>
        /// <param name="newMsg">新消息内容</param>
        /// <returns></returns>
        public TdbResMsgInfo FromNewMsg(string newMsg)
        {
            return new TdbResMsgInfo(this.Code, newMsg);
        }
    }
}
