using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.relationships.repository.DBEntity
{
    /// <summary>
    /// 人际圈信息表
    /// </summary>
    [SugarTable("circle_info")]
    public class CircleInfo
    {
        /// <summary>
        /// 人际圈ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Name { get; set; } = "";

        /// <summary>
        /// 成员数上限
        /// </summary>
        public int MaxMembers { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = 255)]
        public string Remark { get; set; } = "";

        /// <summary>
        /// 创建者ID
        /// </summary>
        public long CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        public long UpdaterID { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
