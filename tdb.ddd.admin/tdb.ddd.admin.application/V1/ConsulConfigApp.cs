using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.application.BusMediatR;
using tdb.ddd.admin.application.contracts.V1.Interface;
using tdb.ddd.admin.domain.ConsulConfig.Aggregate;
using tdb.ddd.admin.domain.contracts.ConsulConfig;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure.Services;

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
            var opeReq = new TdbOperateReq(0, "匿名");
            return await RestoreConfigAsync<CommonConfigInfo>(0, $"consulConfig{Path.DirectorySeparatorChar}common.json", CommonConfigInfo.PrefixKey, opeReq);
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
            var opeReq = new TdbOperateReq(0, "匿名");
            return await RestoreConfigAsync<AccountConfigInfo>(TdbCst.ServerID.Account, $"consulConfig{Path.DirectorySeparatorChar}account.json", AccountConfigInfo.PrefixKey, opeReq);
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
        /// <param name="req">操作人信息</param>
        /// <returns></returns>
        public async Task<TdbRes<string>> RestoreFilesConfigAsync(TdbOperateReq req)
        {
            return await RestoreConfigAsync<FilesConfigInfo>(TdbCst.ServerID.Files, $"consulConfig{Path.DirectorySeparatorChar}files.json", FilesConfigInfo.PrefixKey, req);
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
        /// <param name="req">操作人信息</param>
        /// <returns></returns>
        public async Task<TdbRes<string>> RestoreRelationshipsConfigAsync(TdbOperateReq req)
        {
            return await RestoreConfigAsync<RelationshipsConfigInfo>(TdbCst.ServerID.Relationships, $"consulConfig{Path.DirectorySeparatorChar}relationships.json", RelationshipsConfigInfo.PrefixKey, req);
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
        /// <param name="req">操作人信息</param>
        /// <returns></returns>
        private static async Task<TdbRes<string>> RestoreConfigAsync<T>(int serviceID, string jsonPath, string prefixKey, TdbOperateReq opeOpe) where T : class, new()
        {
            var configConfigInfo = GetConsulConfigInfo();
            var agg = new ConsulConfigAgg<T>()
            {
                ID = serviceID,
                JsonPath = jsonPath,
                ConsulIP = configConfigInfo.IP,
                ConsulPort = configConfigInfo.Port,
                PrefixKey = $"{infrastructure.Config.AdminConfig.App.Server.Environment}.{prefixKey}"
            };
            await agg.RestoreConfigAsync();

            //推送通知
            var msg = new RestoreConfigNotification()
            {
                ServiceID = serviceID,
                JsonPath = jsonPath,
                OperatorID = opeOpe.OperatorID,
                OperationTime = opeOpe.OperationTime
            };
            TdbMediatR.Publish(msg);

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
