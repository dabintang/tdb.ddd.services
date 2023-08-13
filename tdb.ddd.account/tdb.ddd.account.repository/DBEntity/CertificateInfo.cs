using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.account.repository.DBEntity
{
    /// <summary>
    /// 凭证信息表
    /// </summary>
    [SugarTable("certificate_info")]
    public class CertificateInfo
    {
        /// <summary>
        /// 凭证ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 凭证类型（1：指纹）
        /// </summary>
        public byte CertificateTypeCode { get; set; }

        /// <summary>
        /// 凭证内容
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Credentials { get; set; } = "";

        /// <summary>
        /// 创建者ID
        /// </summary>
        public long CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
