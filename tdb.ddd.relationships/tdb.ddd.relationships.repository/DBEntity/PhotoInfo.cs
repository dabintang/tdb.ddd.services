using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.relationships.repository.DBEntity
{
    /// <summary>
    /// 照片信息表
    /// </summary>
    [SugarTable("photo_info")]
    public class PhotoInfo
    {
        /// <summary>
        /// 照片文件ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        public long PersonnelID { get; set; }

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
