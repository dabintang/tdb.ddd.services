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
    /// 获取人际圈信息 请求参数
    /// </summary>
    public class GetCircleReq
    {
        /// <summary>
        /// [必须]人际圈ID
        /// </summary>
        [TdbHashIDModelBinder]
        [TdbRequired("人际圈ID")]
        public long ID { get; set; }
    }

    /// <summary>
    /// 获取人际圈信息 结果
    /// </summary>
    public class GetCircleRes
    {
        /// <summary>
        /// 人际圈ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 成员数上限
        /// </summary>
        public int MaxMembers { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 创建者ID
        /// </summary>
        [TdbHashIDJsonConverter]
        public long CreatorID { get; set; }
    }
}
