using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.account.application.contracts.V1.DTO
{
    /// <summary>
    /// 添加用户结果
    /// </summary>
    public class AddUserRes
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }
    }
}
