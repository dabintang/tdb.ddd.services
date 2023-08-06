using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Circle
{
    /// <summary>
    /// 批量从人际圈移除成员 请求参数
    /// </summary>
    public class BatchRemoveMemberReq
    {
        /// <summary>
        /// [必须]人际圈ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("人际圈ID")]
        public long CircleID { get; set; }

        /// <summary>
        /// [必须]人员ID
        /// </summary>
        [TdbHashIDListJsonConverter]
        [TdbRequired("人员ID")]
        public List<long> LstPersonnelID { get; set; } = new List<long>();
    }

    /// <summary>
    /// 批量从人际圈移除成员 结果
    /// </summary>
    public class BatchRemoveMemberRes
    {
        /// <summary>
        /// 移除成功人数
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 移除失败人数
        /// </summary>
        public int FailCount { get; set; }

        /// <summary>
        /// 失败信息
        /// </summary>
        public List<FailInfo> LstFailInfo { get; set; } = new List<FailInfo>();

        /// <summary>
        /// 失败信息
        /// </summary>
        public class FailInfo
        {
            /// <summary>
            /// 人员ID
            /// </summary>
            [TdbHashIDJsonConverter]
            public long PersonnelID { get; set; }

            /// <summary>
            /// 人员姓名
            /// </summary>
            public string Name { get; set; } = "";

            /// <summary>
            /// 失败原因
            /// </summary>
            public string Reason { get; set; } = "";
        }
    }
}
