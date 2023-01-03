using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 操作条件包装类
    /// </summary>
    public class TdbOperateReq<T>
    {
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

        /// <summary>
        /// 操作人拥有的角色ID
        /// </summary>
        public List<long> OperatorRoleIDs { get; set; } = new List<long>();

        /// <summary>
        /// 操作人拥有的权限ID
        /// </summary>
        public List<long> OperatorAuthorityIDs { get; set; } = new List<long>();

        /// <summary>
        /// 参数
        /// </summary>
        public T? Param { get; set; }
    }
}
