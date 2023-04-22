using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 修改文件状态 消息
    /// </summary>
    public class UpdateFilesStatusMsg
    {
        /// <summary>
        /// [必须]文件状态集合
        /// </summary>
        public List<FileStatus> LstFileStatus { get; set; } = new List<FileStatus>();

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long OperatorID { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperatorName { get; set; } = "";

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; } = DateTime.Now;

        #region 内部类

        /// <summary>
        /// 文件状态
        /// </summary>
        public class FileStatus
        {
            /// <summary>
            /// [必须]文件ID
            /// </summary>
            public long ID { get; set; }

            /// <summary>
            /// 文件状态（1：临时文件；2：正式文件）
            /// </summary>
            public EnmTdbFileStatus FileStatusCode { get; set; }
        }

        #endregion
    }

    /// <summary>
    /// 修改文件状态 消息
    /// </summary>
    public class UpdateFilesStatusRes
    {
        /// <summary>
        /// 更新结果集合
        /// </summary>
        public List<UpdateResult> LstResult { get; set; } = new List<UpdateResult>();

        #region 内部类

        /// <summary>
        /// 更新结果
        /// </summary>
        public class UpdateResult
        {
            /// <summary>
            /// 文件ID
            /// </summary>
            public long ID { get; set; }

            /// <summary>
            /// 是否更新成功
            /// </summary>
            public bool IsSuccess { get; set; }

            /// <summary>
            /// 结果信息
            /// </summary>
            public string Msg { get; set; } = "";
        }

        #endregion
    }
}
