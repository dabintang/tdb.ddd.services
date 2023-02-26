using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.admin.application.contracts.Remote
{
    /// <summary>
    /// 账户服务应用接口
    /// </summary>
    public interface IAccountApp : ITdbIOCScoped
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        Task<TdbRes<string>> InitDataAsync();
    }
}
