using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.files.infrastructure.Config;
using tdb.ddd.infrastructure;

namespace tdb.ddd.files.infrastructure
{
    /// <summary>
    /// 生产唯一编码帮助类
    /// </summary>
    public class FilesUniqueIDHelper
    {
        /// <summary>
        /// 生成唯一ID
        /// </summary>
        /// <returns></returns>
        public static long CreateID()
        {
            return TdbUniqueIDHelper.CreateID(TdbCst.ServerID.Files, FilesConfig.App.Server.Seq);
        }
    }
}
