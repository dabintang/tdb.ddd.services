using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.infrastructure.Config;

namespace tdb.ddd.relationships.infrastructure
{
    /// <summary>
    /// 生产唯一编码帮助类
    /// </summary>
    public class RelationshipsUniqueIDHelper
    {
        /// <summary>
        /// 生成唯一ID
        /// </summary>
        /// <returns></returns>
        public static long CreateID()
        {
            return TdbUniqueIDHelper.CreateID(TdbCst.ServerID.Files, RelationshipsConfig.App.Server.Seq);
        }
    }
}
