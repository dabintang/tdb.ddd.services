using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Personnel
{
    /// <summary>
    /// 创建我的人员信息 条件
    /// </summary>
    public class CreateMyPersonnelInfoReq
    {
    }

    /// <summary>
    /// 创建我的人员信息 结果
    /// </summary>
    public class CreateMyPersonnelInfoRes
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }
    }
}
