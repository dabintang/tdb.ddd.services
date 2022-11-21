using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 异常
    /// </summary>
    public class TdbException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        public TdbException(string message) : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public TdbException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
