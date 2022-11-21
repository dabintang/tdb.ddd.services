using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 非法参数信息
    /// </summary>
    public class TdbInvalidParamInfo
    {
        /// <summary>
        /// 特性类型
        /// </summary>
        public Type AttrType { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="attrType">特性类型</param>
        /// <param name="msg">错误消息</param>
        public TdbInvalidParamInfo(Type attrType, string msg)
        {
            this.AttrType = attrType;
            this.Msg = msg;
        }
    }
}
