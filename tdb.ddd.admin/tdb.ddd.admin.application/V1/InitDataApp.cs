using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.application.contracts.Remote;
using tdb.ddd.admin.application.contracts.V1.Interface;
using tdb.ddd.admin.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;

namespace tdb.ddd.admin.application.V1
{
    /// <summary>
    /// 初始化数据应用
    /// </summary>
    public class InitDataApp : IInitDataApp
    {
        #region 实现接口

        /// <summary>
        /// 初始化账户服务数据
        /// </summary>
        /// <param name="secretKey">秘钥</param>
        /// <returns></returns>
        public async Task<TdbRes<string>> InitAccountDataAsync(string secretKey)
        {
            if (secretKey != AdminConfig.Common.Token.SecretKey)
            {
                return TdbRes.Fail("口令不正确");
            }

            //调用账户服务初始化数据接口 
            var accountApp = TdbIOC.GetService<IAccountApp>();
            var res = await accountApp.InitDataAsync();

            return res;
        }

        #endregion
    }
}
