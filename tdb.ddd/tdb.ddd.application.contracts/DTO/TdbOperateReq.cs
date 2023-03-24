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
    public class TdbOperateReq<T> : TdbOperateReq
    {
        /// <summary>
        /// 参数
        /// </summary>
        public T Param { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="param">条件</param>
        /// <param name="operatorID">操作人ID</param>
        /// <param name="operatorName">操作人姓名</param>
        public TdbOperateReq(T param, long operatorID, string operatorName) : base(operatorID, operatorName)
        {
            this.Param = param;
        }
    }

    /// <summary>
    /// 操作条件
    /// </summary>
    public class TdbOperateReq
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
        /// 构造函数
        /// </summary>
        /// <param name="operatorID">操作人ID</param>
        /// <param name="operatorName">操作人姓名</param>
        public TdbOperateReq(long operatorID, string operatorName)
        {
            this.OperatorID = operatorID;
            this.OperatorName = operatorName;
        }
    }
}
