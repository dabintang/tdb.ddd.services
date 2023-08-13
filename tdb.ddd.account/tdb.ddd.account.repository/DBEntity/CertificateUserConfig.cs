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
    [SugarTable("certificate_user_config")]
    public class CertificateUserConfig
    {
        /// <summary>
        /// 凭证ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long CertificateID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long UserID { get; set; }
    }
}
