using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.admin.infrastructure.Config;
using tdb.ddd.contracts;

namespace tdb.ddd.admin.infrastructure
{
    /// <summary>
    /// 生产唯一编码帮助类
    /// </summary>
    public class AdminUniqueIDHelper
    {
        /// <summary>
        /// 生成唯一ID
        /// </summary>
        /// <returns></returns>
        public static long CreateID()
        {
            return UniqueIDHelper.CreateID(TdbCst.ServerID.Admin, AdminConfig.App.Server.Seq);
        }
    }
}
