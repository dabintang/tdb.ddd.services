using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.domain
{
    /// <summary>
    /// 更新信息值对象
    /// </summary>
    public class UpdateInfoValueObject : TdbValueObject
    {
        /// <summary>
        /// 更新者ID
        /// </summary>
        public long UpdaterID { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
