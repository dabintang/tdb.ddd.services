using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 把ID列表hash加密的json转换特效
    /// </summary>
    public class TdbHashIDListJsonConverterAttribute : JsonConverterAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TdbHashIDListJsonConverterAttribute() : base(typeof(TdbHashIDListJsonConverter))
        {

        }
    }
}
