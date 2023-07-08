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
    /// 查询我创建的人员信息列表 条件
    /// </summary>
    public class QueryMyPersonnelListReq : TdbPageReqBase<QueryMyPersonnelListReq.EnmSortField>
    {
        /// <summary>
        /// 排序字段（1：人员ID；2：姓名）
        /// </summary>
        public enum EnmSortField
        {
            /// <summary>
            /// 人员ID
            /// </summary>
            PersonnelID = 1,

            /// <summary>
            /// 姓名
            /// </summary>
            Name = 2
        }
    }

    /// <summary>
    /// 查询我创建的人员信息列表 结果
    /// </summary>
    public class QueryMyPersonnelListRes
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }

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
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; } = "";

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";
    }
}
