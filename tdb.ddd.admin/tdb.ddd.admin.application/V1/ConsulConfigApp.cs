using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.application.contracts.V1.Interface;
using tdb.ddd.admin.domain.ConsulConfig.Aggregate;
using tdb.ddd.admin.domain.contracts.ConsulConfig;
using tdb.ddd.contracts;

namespace tdb.ddd.admin.application.V1
{
    /// <summary>
    /// consul配置应用
    /// </summary>
    public class ConsulConfigApp : IConsulConfigApp
    {
        #region 实现接口

        /// <summary>
        /// 还原共用配置
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> RestoreCommonConfigAsync()
        {
            return await RestoreConfigAsync<CommonConfigInfo>(0, $"consulConfig{Path.DirectorySeparatorChar}common.json", CommonConfigInfo.PrefixKey);
        }

        /// <summary>
        /// 备份共用配置
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> BackupCommonConfigAsync()
        {
            return await BackupConfigAsync<CommonConfigInfo>(0, $"consulConfig{Path.DirectorySeparatorChar}common.json", CommonConfigInfo.PrefixKey);
        }

        /// <summary>
        /// 还原账户服务配置
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> RestoreAccountConfigAsync()
        {
            return await RestoreConfigAsync<AccountConfigInfo>(TdbCst.ServerID.Account, $"consulConfig{Path.DirectorySeparatorChar}account.json", AccountConfigInfo.PrefixKey);
        }

        /// <summary>
        /// 备份账户服务配置
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> BackupAccountConfigAsync()
        {
            return await BackupConfigAsync<AccountConfigInfo>(TdbCst.ServerID.Account, $"consulConfig{Path.DirectorySeparatorChar}account.json", AccountConfigInfo.PrefixKey);
        }

        /// <summary>
        /// 还原文件服务配置
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> RestoreFilesConfigAsync()
        {
            return await RestoreConfigAsync<FilesConfigInfo>(TdbCst.ServerID.Files, $"consulConfig{Path.DirectorySeparatorChar}files.json", FilesConfigInfo.PrefixKey);
        }

        /// <summary>
        /// 备份文件服务配置
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> BackupFilesConfigAsync()
        {
            return await BackupConfigAsync<FilesConfigInfo>(TdbCst.ServerID.Files, $"consulConfig{Path.DirectorySeparatorChar}files.json", FilesConfigInfo.PrefixKey);
        }

        /// <summary>
        /// 还原人际关系服务配置
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> RestoreRelationshipsConfigAsync()
        {
            return await RestoreConfigAsync<RelationshipsConfigInfo>(TdbCst.ServerID.Relationships, $"consulConfig{Path.DirectorySeparatorChar}relationships.json", RelationshipsConfigInfo.PrefixKey);
        }

        /// <summary>
        /// 备份文件服务配置
        /// </summary>
        /// <returns></returns>
        public async Task<TdbRes<string>> BackupRelationshipsConfigAsync()
        {
            return await BackupConfigAsync<RelationshipsConfigInfo>(TdbCst.ServerID.Relationships, $"consulConfig{Path.DirectorySeparatorChar}relationships.json", RelationshipsConfigInfo.PrefixKey);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 还原配置
        /// </summary>
        /// <typeparam name="T">配置信息类型</typeparam>
        /// <param name="serviceID">服务ID</param>
        /// <param name="jsonPath">json文件路径</param>
        /// <param name="prefixKey">配置key前缀（一般用来区分不同服务）</param>
        /// <returns></returns>
        private static async Task<TdbRes<string>> RestoreConfigAsync<T>(int serviceID, string jsonPath, string prefixKey) where T : class, new()
        {
            var configConfigInfo = GetConsulConfigInfo();
            var agg = new ConsulConfigAgg<T>()
            {
                ID = serviceID,
                JsonPath = jsonPath,
                ConsulIP = configConfigInfo.IP,
                ConsulPort = configConfigInfo.Port,
                PrefixKey = prefixKey
            };
            await agg.RestoreConfigAsync();

            return TdbRes.Success($"已还原配置：{jsonPath}");
        }

        /// <summary>
        /// 备份配置
        /// </summary>
        /// <typeparam name="T">配置信息类型</typeparam>
        /// <param name="serviceID">服务ID</param>
        /// <param name="jsonPath">json文件路径</param>
        /// <param name="prefixKey">配置key前缀（一般用来区分不同服务）</param>
        /// <returns></returns>
        private static async Task<TdbRes<string>> BackupConfigAsync<T>(int serviceID, string jsonPath, string prefixKey) where T : class, new()
        {
            var configConfigInfo = GetConsulConfigInfo();
            var agg = new ConsulConfigAgg<T>()
            {
                ID = serviceID,
                JsonPath = jsonPath,
                ConsulIP = configConfigInfo.IP,
                ConsulPort = configConfigInfo.Port,
                PrefixKey = prefixKey
            };
            var backupFullFileName = await agg.BackupConfigAsync();

            return TdbRes.Success($"已备份配置到：{backupFullFileName}");
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取consul配置
        /// </summary>
        /// <returns></returns>
        private static infrastructure.Config.AppConfigInfo.AppConsulConfigInfo GetConsulConfigInfo()
        {
            return infrastructure.Config.AdminConfig.App.Consul;
        }

        #endregion
    }
}
