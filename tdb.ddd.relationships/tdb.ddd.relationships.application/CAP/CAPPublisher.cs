using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.relationships.application.CAP
{
    /// <summary>
    /// cap发布者
    /// </summary>
    public class CAPPublisher
    {
        /// <summary>
        /// 更新文件状态
        /// </summary>
        /// <param name="msg">cap消息</param>
        /// <returns></returns>
        public static async Task UpdateFilesStatusAsync(UpdateFilesStatusMsg msg)
        {
            await TdbCAP.PublishAsync(TdbCst.CAPTopic.UpdateFilesStatus, msg);
        }
    }
}
