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
    /// 添加人员照片参数
    /// </summary>
    public class AddPersonnelPhotoReq
    {
        /// <summary>
        /// [必须]人员ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("人员ID")]
        public long PersonnelID { get; set; }

        /// <summary>
        /// [必须]照片ID集合
        /// </summary>
        [TdbHashIDListJsonConverter]
        [TdbRequired("照片ID集合")]
        public List<long> LstPhotoID { get; set; } = new List<long>();
    }
}
