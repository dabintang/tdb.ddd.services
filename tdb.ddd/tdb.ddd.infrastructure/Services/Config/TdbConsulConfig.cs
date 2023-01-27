using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.consul.kv;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 分布式配置接口实现
    /// </summary>
    public class TdbConsulConfig : ITdbDistributedConfig
    {
        /// <summary>
        /// consul服务IP
        /// </summary>
        private string ConsulIP { get; set; }

        /// <summary>
        /// consul服务端口
        /// </summary>
        private int ConsulPort { get; set; }

        /// <summary>
        /// key前缀，一般用来区分不同服务
        /// </summary>
        private string PrefixKey { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="consulIP">consul服务IP</param>
        /// <param name="consulPort">consul服务端口</param>
        /// <param name="prefixKey">key前缀，一般用来区分不同服务</param>
        public TdbConsulConfig(string consulIP, int consulPort, string prefixKey)
        {
            this.ConsulIP = consulIP;
            this.ConsulPort = consulPort;
            this.PrefixKey = prefixKey;
        }

        #region 实现接口

        /// <summary>
        /// 获取consul上的配置信息
        /// </summary>
        /// <typeparam name="T">consul配置信息类型</typeparam>
        /// <returns></returns>
        public async Task<T> GetConfigAsync<T>() where T : class, new()
        {
            return await ConsulConfigHelper.GetConfigAsync<T, TdbConfigKeyAttribute>(this.ConsulIP, this.ConsulPort, this.PrefixKey);
        }

        /// <summary>
        /// 备份配置
        /// </summary>
        /// <typeparam name="T">consul配置信息类型</typeparam>
        /// <param name="fullFileName">完整备份文件名(.json文件)</param>
        /// <returns>完整备份文件名</returns>
        public async Task<string> BackupConfigAsync<T>(string fullFileName = "") where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                fullFileName = CommHelper.GetFullFileName($"backup{Path.DirectorySeparatorChar}consulConfig_{DateTime.Now:yyyyMMddHHmmssfff}.json");
            }

            var path = Path.GetDirectoryName(fullFileName) ?? "";
            //如果路径不存在，创建路径
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            //获取consul上的配置信息
            var config = await this.GetConfigAsync<T>();

            //转json字符串
            var jsonTxt = JsonConvert.SerializeObject(config);

            //写文件
            File.WriteAllText(fullFileName, jsonTxt, Encoding.Default);

            return fullFileName;
        }

        /// <summary>
        /// 还原配置
        /// </summary>
        /// <typeparam name="T">consul配置信息类型</typeparam>
        /// <param name="config">配置信息</param>
        public async Task RestoreConfigAsync<T>(T config) where T : class, new()
        {
            //还原
            await ConsulConfigHelper.PutConfig<T, TdbConfigKeyAttribute>(this.ConsulIP, this.ConsulPort, config, this.PrefixKey);
        }

        #endregion

        #region 静态方法

        ///// <summary>
        ///// 获取consul上的配置信息
        ///// </summary>
        ///// <typeparam name="T">consul配置信息类型</typeparam>
        ///// <param name="consulIP">consul服务IP</param>
        ///// <param name="consulPort">consul服务端口</param>
        ///// <param name="prefixKey">key前缀，一般用来区分不同服务</param>
        ///// <returns>consul配置信息</returns>
        //private static async Task<T> GetConfigAsync<T>(string consulIP, int consulPort, string prefixKey) where T : class, new()
        //{
        //    Dictionary<string, string> dicPair;
        //    using (var kv = new ConsulKV(consulIP, consulPort, prefixKey))
        //    {
        //        //获取所有key/value
        //        dicPair = await kv.ListAsync();
        //    }

        //    //创建对象
        //    var obj = new T();

        //    Type type = typeof(T);
        //    //获取对象属性
        //    var pros = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    //给对象属性赋值
        //    foreach (var pro in pros)
        //    {
        //        //特性
        //        var attr = pro.GetCustomAttributes<TdbConfigKeyAttribute>().FirstOrDefault();
        //        var key = $"{prefixKey}{attr?.Key}";
        //        if (attr == null || dicPair.ContainsKey(key) == false)
        //        {
        //            continue;
        //        }

        //        //字符串值
        //        var strValue = dicPair[key];

        //        //如果是字符串类型
        //        if (pro.PropertyType == typeof(string))
        //        {
        //            CommHelper.EmitSet(obj, pro.Name, strValue);
        //        }
        //        //如果是日期类型
        //        else if (pro.PropertyType == typeof(DateTime))
        //        {
        //            var value = Convert.ToDateTime(strValue);
        //            CommHelper.EmitSet(obj, pro.Name, value);
        //        }
        //        else
        //        {
        //            var value = strValue.DeserializeJson(pro.PropertyType);
        //            CommHelper.EmitSet(obj, pro.Name, value);
        //        }
        //    }

        //    return obj;
        //}

        ///// <summary>
        ///// 设置配置信息
        ///// </summary>
        ///// <typeparam name="T">consul配置信息类型</typeparam>
        ///// <param name="consulIP">consul服务IP</param>
        ///// <param name="consulPort">consul服务端口</param>
        ///// <param name="config">consul配置信息</param>
        ///// <param name="prefixKey">key前缀，一般用来区分不同服务</param>
        ///// <returns></returns>
        //private static async Task<bool> PutConfig<T>(string consulIP, int consulPort, T config, string prefixKey) where T : class
        //{
        //    //配置信息字典
        //    var dicConfig = new Dictionary<string, object?>();

        //    Type type = typeof(T);
        //    //获取对象属性
        //    var pros = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    if (pros == null)
        //    {
        //        return true;
        //    }

        //    //找出有配置特性的属性，并把属性对应放入字典
        //    foreach (var pro in pros)
        //    {
        //        //特性
        //        var attr = pro.GetCustomAttributes<TdbConfigKeyAttribute>().FirstOrDefault();
        //        if (attr == null)
        //        {
        //            continue;
        //        }

        //        dicConfig[attr.Key] = CommHelper.EmitGet(config, pro.Name);
        //    }

        //    using var kv = new ConsulKV(consulIP, consulPort, prefixKey);
        //    //写入consul
        //    return await kv.PutAllAsync(dicConfig);
        //}

        #endregion
    }
}
