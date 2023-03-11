using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.admin.application.contracts.V1.Interface
{
    /// <summary>
    /// 初始化数据应用接口
    /// </summary>
    public interface IInitDataApp : ITdbIOCScoped
    {
        /// <summary>
        /// 初始化账户服务数据
        /// </summary>
        /// <param name="secretKey">秘钥</param>
        /// <returns></returns>
        Task<TdbRes<string>> InitAccountDataAsync();
    }
}
