using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.ddd.files.application.contracts.V1.DTO
{
    /// <summary>
    /// 确认文件 条件参数
    /// </summary>
    public class ConfirmFileReq
    {
        /// <summary>
        /// [必须]文件ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("文件ID")]
        public long ID { get; set; }
    }
}
