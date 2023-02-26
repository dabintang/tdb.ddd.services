using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.account.application.remote.Files.DTO
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
        public long ID { get; set; }
    }
}
