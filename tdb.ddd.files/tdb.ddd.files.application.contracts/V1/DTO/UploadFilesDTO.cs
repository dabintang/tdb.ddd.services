using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.files.domain.contracts.Enum;

namespace tdb.ddd.files.application.contracts.V1.DTO
{
    /// <summary>
    /// 上传文件 请求参数
    /// </summary>
    public class UploadFilesReq
    {
        /// <summary>
        /// 文件名（含后缀）
        /// </summary>           
        public string Name { get; set; }

        /// <summary>
        /// 访问级别(1：仅创建者；2：授权；3：公开)
        /// </summary>
        public EnmAccessLevel AccessLevelCode { get; set; }

        /// <summary>
        /// 存储类型（1：本地磁盘）
        /// </summary>
        public EnmStorageType StorageTypeCode { get; set; }

        /// <summary>
        /// 文件状态（1：临时文件；2：正式文件）
        /// </summary>
        public EnmFileStatus FileStatusCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 文件数据
        /// </summary>
        public byte[] Data { get; set; }
    }

    /// <summary>
    /// 上传文件 结果
    /// </summary>
    public class UploadFilesRes
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否上传成功
        /// </summary>
        public bool IsOK { get; set; }

        /// <summary>
        /// 结果描述
        /// </summary>
        public string Msg { get; set; }
    }
}
