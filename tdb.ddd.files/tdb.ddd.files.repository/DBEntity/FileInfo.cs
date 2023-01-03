using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;
using tdb.ddd.files.domain.contracts.Enum;

namespace tdb.ddd.files.repository.DBEntity
{
    /// <summary>
    /// 文件信息表
    /// </summary>
    [SugarTable("file_info")]
    public class FileInfo
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long ID { get; set; }

        /// <summary>
        /// 文件名（含后缀）
        /// </summary>
        [SugarColumn(Length = 255)]
        public string Name { get; set; }

        /// <summary>
        /// 文件地址(本地路径或url)
        /// </summary>
        [SugarColumn(Length = 255)]
        public string Address { get; set; }

        /// <summary>
        /// 存储类型（1：本地磁盘）
        /// </summary>
        public byte StorageTypeCode { get; set; }

        /// <summary>
        /// 字节数
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 访问级别(1：仅创建者；2：授权；3：公开)
        /// </summary>
        public byte AccessLevelCode { get; set; }

        /// <summary>
        /// 文件状态（1：临时文件；2：正式文件）
        /// </summary>
        public byte FileStatusCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = 255)]
        public string Remark { get; set; }

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
