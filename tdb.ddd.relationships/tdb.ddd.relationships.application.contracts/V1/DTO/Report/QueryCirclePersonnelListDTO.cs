using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.relationships.domain.contracts.Enum;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Report
{
    /// <summary>
    /// 查询人际圈内人员信息列表 条件
    /// </summary>
    public class QueryCirclePersonnelListReq : TdbPageReqBase<QueryCirclePersonnelListReq.EnmSortField>
    {
        /// <summary>
        /// [必须]人际圈ID
        /// </summary>
        [TdbHashIDModelBinder]
        [TdbRequired("人际圈ID")]
        public long CircleID { get; set; }

        /// <summary>
        /// 排序字段（1：角色编码；2：姓名）
        /// </summary>
        public enum EnmSortField
        {
            /// <summary>
            /// 角色编码
            /// </summary>
            Role = 1,

            /// <summary>
            /// 姓名
            /// </summary>
            Name = 2
        }
    }

    /// <summary>
    /// 查询人际圈内人员信息列表 结果
    /// </summary>
    public class QueryCirclePersonnelListRes
    {
        /// <summary>
        /// 人际圈ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long CircleID { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long PersonnelID { get; set; }

        /// <summary>
        /// 头像图片ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long? HeadImgID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 性别
        /// </summary>
        public EnmGender GenderCode { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public EnmRole RoleCode { get; set; }

        /// <summary>
        /// 圈内身份
        /// </summary>
        public string Identity { get; set; } = "";

        /// <summary>
        /// 人员创建者ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long PersonnelCreatorID { get; set; }
    }
}
