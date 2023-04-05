using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.admin.application.contracts.V1.Interface
{
    /// <summary>
    /// consul配置应用接口
    /// </summary>
    public interface IConsulConfigApp : ITdbIOCScoped
    {
        /// <summary>
        /// 还原共用配置
        /// </summary>
        /// <returns></returns>
        Task<TdbRes<string>> RestoreCommonConfigAsync();

        /// <summary>
        /// 备份共用配置
        /// </summary>
        /// <returns></returns>
        Task<TdbRes<string>> BackupCommonConfigAsync();

        /// <summary>
        /// 还原账户服务配置
        /// </summary>
        /// <returns></returns>
        Task<TdbRes<string>> RestoreAccountConfigAsync();

        /// <summary>
        /// 备份账户服务配置
        /// </summary>
        /// <returns></returns>
        Task<TdbRes<string>> BackupAccountConfigAsync();

        /// <summary>
        /// 还原文件服务配置
        /// </summary>
        /// <returns></returns>
        Task<TdbRes<string>> RestoreFilesConfigAsync();

        /// <summary>
        /// 备份文件服务配置
        /// </summary>
        /// <returns></returns>
        Task<TdbRes<string>> BackupFilesConfigAsync();

        /// <summary>
        /// 还原人际关系服务配置
        /// </summary>
        /// <returns></returns>
        Task<TdbRes<string>> RestoreRelationshipsConfigAsync();

        /// <summary>
        /// 备份人际关系服务配置
        /// </summary>
        /// <returns></returns>
        Task<TdbRes<string>> BackupRelationshipsConfigAsync();
    }
}
