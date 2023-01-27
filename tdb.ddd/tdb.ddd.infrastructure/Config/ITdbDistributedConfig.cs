using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// 分布式配置接口
    /// </summary>
    public interface ITdbDistributedConfig
    {
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <typeparam name="T">配置信息类型</typeparam>
        /// <returns></returns>
        Task<T> GetConfigAsync<T>() where T : class, new();

        /// <summary>
        /// 备份配置
        /// </summary>
        /// <typeparam name="T">配置信息类型</typeparam>
        /// <param name="fullFileName">完整备份文件名(.json文件)</param>
        /// <returns>完整备份文件名</returns>
        Task<string> BackupConfigAsync<T>(string fullFileName = "") where T : class, new();

        /// <summary>
        /// 还原配置
        /// </summary>
        /// <typeparam name="T">配置信息类型</typeparam>
        /// <param name="config">配置信息</param>
        Task RestoreConfigAsync<T>(T config) where T : class, new();
    }
}
