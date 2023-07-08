using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Report
{
    /// <summary>
    /// 查询人际圈列表 参数
    /// </summary>
    public class QueryCircleListReq : TdbPageReqBase<QueryCircleListReq.EnmSortField>
    {
        /// <summary>
        /// [可选]人员ID（查询该人员加入的人际圈）
        /// </summary>
        [TdbHashIDModelBinder]
        public long? PersonnelID { get; set; }

        /// <summary>
        /// [可选]人际圈名称（模糊匹配）
        /// </summary>
        public string? CircleName { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public override List<TdbSortItem<EnmSortField>>? LstSortItem { get; set; }

        /// <summary>
        /// 排序字段（1：人际圈ID；2：人际圈名称）
        /// </summary>
        public enum EnmSortField
        {
            /// <summary>
            /// 人际圈ID
            /// </summary>
            ID = 1,

            /// <summary>
            /// 人际圈名称
            /// </summary>
            CircleName = 2
        }
    }

    /// <summary>
    /// 查询人际圈列表 结果
    /// </summary>
    public class QueryCircleListRes
    {
        /// <summary>
        /// 人际圈ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }

        /// <summary>
        /// 人际圈名称
        /// </summary>
        public string CircleName { get; set; } = "";

        /// <summary>
        /// 成员数上限
        /// </summary>
        public int MaxMembers { get; set; }

        /// <summary>
        /// 成员数
        /// </summary>
        public int MembersCount { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long CreatorID { get; set; }
    }
}
