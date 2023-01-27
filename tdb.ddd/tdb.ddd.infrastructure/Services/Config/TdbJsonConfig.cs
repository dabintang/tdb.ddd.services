using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using tdb.common;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// json配置接口实现
    /// </summary>
    public class TdbJsonConfig : ITdbJsonConfig
    {
        /// <summary>
        /// 配置
        /// </summary>
        private readonly IConfigurationRoot configuration;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configureSource">配置源</param>
        public TdbJsonConfig(Action<JsonConfigurationSource> configureSource)
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile(configureSource).Build();

            //监听配置改动
            ChangeToken.OnChange(() => configuration.GetReloadToken(), OnConfigReload, configuration);
        }

        #region 实现接口

        /// <summary>
        /// 获取appsettings.json配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfig<T>() where T : class, new()
        {
            //创建对象
            var obj = new T();

            //设置appsettings.json配置信息
            SetConfig(obj);

            return obj;
        }

        /// <summary>
        /// 配置重新加载事件
        /// </summary>
        public event DelegateConfigReload? ConfigReload;

        #endregion

        #region 私有方法

        /// <summary>
        /// 设置配置信息
        /// </summary>
        /// <param name="obj">配置信息对象</param>
        private void SetConfig(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            //获取对象属性
            var pros = obj.GetType().GetProperties();
            //给对象属性赋值
            foreach (var pro in pros)
            {
                if (pro == null || pro.PropertyType == null || pro.PropertyType.FullName == null)
                {
                    continue;
                }

                //特性
                var attr = pro.GetCustomAttributes<TdbConfigKeyAttribute>().FirstOrDefault();
                if (attr == null)
                {
                    if (pro.PropertyType != typeof(string) && pro.PropertyType.IsClass && pro.PropertyType.IsPrimitive == false)
                    {
                        var proVal = pro.GetValue(obj);
                        if (proVal == null)
                        {
                            proVal = Activator.CreateInstance(pro.PropertyType);
                            CommHelper.ReflectSet(obj, pro.Name, proVal);
                        }
                        SetConfig(proVal);
                    }

                    continue;
                }
                
                //字符串值
                var strValue = configuration[attr.Key];

                //如果是字符串类型
                if (pro.PropertyType == typeof(string))
                {
                    CommHelper.ReflectSet(obj, pro.Name, strValue);
                }
                //如果是日期类型
                else if (pro.PropertyType == typeof(DateTime))
                {
                    var value = Convert.ToDateTime(strValue);
                    CommHelper.ReflectSet(obj, pro.Name, value);
                }
                else if (pro.PropertyType.IsClass)
                {
                    var value = pro.PropertyType.Assembly.CreateInstance(pro.PropertyType.FullName);
                    configuration.Bind(attr.Key, value);
                    CommHelper.ReflectSet(obj, pro.Name, value);
                }
                else
                {
                    var value = strValue.DeserializeJson(pro.PropertyType);
                    CommHelper.ReflectSet(obj, pro.Name, value);
                }
            }
        }

        /// <summary>
        /// 配置重新加载
        /// </summary>
        /// <param name="config"></param>
        private void OnConfigReload(IConfigurationRoot config)
        {
            this.ConfigReload?.Invoke();
        }

        #endregion
    }
}
