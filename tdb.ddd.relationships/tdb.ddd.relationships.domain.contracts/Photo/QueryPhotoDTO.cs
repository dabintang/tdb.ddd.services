using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.domain.contracts.Photo
{
    /// <summary>
    /// 查询照片参数
    /// </summary>
    public class QueryPhotoParam : TdbPageReqBase
    {
        /// <summary>
        /// [必须]人员ID
        /// </summary>
        public long PersonnelID { get; set; }

        /// <summary>
        /// [可选]人际圈ID
        /// </summary>
        public long? CircleID { get; set; }
    }
}
