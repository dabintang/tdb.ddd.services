using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.account.application.contracts
{
    /// <summary>
    /// 工具应用接口
    /// </summary>
    public interface IToolsApp : ITdbIOCScoped
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns>日志</returns>
        Task<string> InitDataAsync();
    }
}
