using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.account.application.contracts.CAP
{
    /// <summary>
    /// 用户登录消息
    /// </summary>
    public class UserLoginMsg
    {
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP { get; set; } = "";

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long OperatorID { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperatorName { get; set; } = "";

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; } = DateTime.Now;
    }
}
