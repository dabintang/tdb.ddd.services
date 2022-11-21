using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.domain
{
    /// <summary>
    /// 创建信息值对象
    /// </summary>
    public class CreateInfoValueObject : TdbValueObject
    {
        /// <summary>
        /// 创建者ID
        /// </summary>
        public long CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
