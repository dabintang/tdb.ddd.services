using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;

namespace tdb.ddd.relationships.application.contracts.V1.DTO.Circle
{
    /// <summary>
    /// 添加人际圈 请求参数
    /// </summary>
    public class AddCircleReq
    {
        /// <summary>
        /// [必须]名称
        /// </summary>
        [TdbRequired("名称")]
        [TdbStringLength("名称", 32)]
        public string Name { get; set; } = "";

        /// <summary>
        /// [可选]图标ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long? ImageID { get; set; }

        /// <summary>
        /// [可选]备注
        /// </summary>
        [TdbStringLength("备注", 255)]
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 添加人际圈 结果
    /// </summary>
    public class AddCircleRes
    {
        /// <summary>
        /// 人际圈ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }
    }
}
