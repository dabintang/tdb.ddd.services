using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;

namespace tdb.ddd.admin.application.contracts.V1.DTO
{
    /// <summary>
    /// 初始化账户微服务数据 请求条件
    /// </summary>
    public class InitAccountDataReq
    {
        /// <summary>
        /// 口令
        /// </summary>
        [TdbRequired("口令")]
        public string Secret { get; set; }
    }
}
