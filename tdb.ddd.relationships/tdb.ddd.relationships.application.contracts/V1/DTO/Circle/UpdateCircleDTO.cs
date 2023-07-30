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
    /// 更新人际圈 请求参数
    /// </summary>
    public class UpdateCircleReq
    {
        /// <summary>
        /// [必须]人际圈ID
        /// </summary>
        [TdbHashIDJsonConverter]
        [TdbRequired("人际圈ID")]
        public long ID { get; set; }

        /// <summary>
        /// [必须]名称
        /// </summary>
        [TdbRequired("名称")]
        [TdbStringLength("名称", 32)]
        public string Name { get; set; } = "";

        /// <summary>
        /// [可选]人际圈图标ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long? ImageID { get; set; }

        /// <summary>
        /// [可选]备注
        /// </summary>
        [TdbStringLength("备注", 255)]
        public string? Remark { get; set; }
    }
}
