using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.ddd.domain;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.admin.domain.ConsulConfig.Aggregate
{
    /// <summary>
    /// consul配置聚合
    /// </summary>
    /// <typeparam name="T">配置信息类型</typeparam>
    public class ConsulConfigAgg<T> : TdbAggregateRoot<int> where T : class, new()
    {
        #region 值

        /// <summary>
        /// json文件路径
        /// </summary>
        public string JsonPath { get; set; } = "";

        /// <summary>
        /// consul IP
        /// </summary>
        public string ConsulIP { get; set; } = "";

        /// <summary>
        /// consul端口
        /// </summary>
        public int ConsulPort { get; set; }

        /// <summary>
        /// 配置key前缀（一般用来区分不同服务）
        /// </summary>
        public string PrefixKey { get; set; } = "";

        #endregion

        #region 行为

        /// <summary>
        /// 把json文件上的配置还原到consul
        /// </summary>
        public async Task RestoreConfigAsync()
        {
            //json配置服务
            var jsonConfigService = TdbConfigFactory.CreateJsonConfig(cfg =>
            {
                cfg.Path = this.JsonPath;
                cfg.ReloadOnChange = false;
                cfg.Optional = false;
            });

            //获取json配置信息
            var config = jsonConfigService.GetConfig<T>();

            //consul配置服务
            var consulConfigService = TdbConfigFactory.CreateConsulConfig(this.ConsulIP, this.ConsulPort, this.PrefixKey);
            //还原配置
            await consulConfigService.RestoreConfigAsync(config);
        }

        /// <summary>
        /// 备份配置
        /// </summary>
        /// <param name="fullFileName">完整备份文件名(.json文件)</param>
        /// <returns>完整备份文件名</returns>
        public async Task<string> BackupConfigAsync(string fullFileName = "")
        {
            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                fullFileName = this.GetBackupFileName();
            }

            //consul配置服务
            var consulConfigService = TdbConfigFactory.CreateConsulConfig(this.ConsulIP, this.ConsulPort, this.PrefixKey);
            //备份配置
            return await consulConfigService.BackupConfigAsync<T>(fullFileName);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取备份文件名
        /// </summary>
        /// <returns></returns>
        private string GetBackupFileName()
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.JsonPath);
            var extension = Path.GetExtension(this.JsonPath);
            var fileName = $"backup_consul_config{Path.DirectorySeparatorChar}{fileNameWithoutExtension}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
            return CommHelper.GetFullFileName(fileName);
        }

        #endregion
    }
}
