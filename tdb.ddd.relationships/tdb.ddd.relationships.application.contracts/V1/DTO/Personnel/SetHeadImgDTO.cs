using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Personnel
{
    /// <summary>
    /// 设置人员头像照片 条件
    /// </summary>
    public class SetHeadImgReq
    {
        /// <summary>
        /// [必须]人员ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("人员ID")]
        public long PersonnelID { get; set; }

        /// <summary>
        /// [可选]头像图片ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long? HeadImgID { get; set; }
    }
}
