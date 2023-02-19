using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using tdb.nlog;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// nlog日志
    /// </summary>
    public class TdbNLog : ITdbLog
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly NLogger log;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configFile">配置文件</param>
        public TdbNLog(string configFile = "")
        {
            this.log = new NLogger(configFile);
        }

        #region 实现接口

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="msg">日志内容</param>
        public void Log(EnmTdbLogLevel level, string msg)
        {
            //日志级别转换
            LogLevel logLevel = CvtLogLevel(level);

            this.log.Log(logLevel, AddUniqueID(msg));
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="exception">异常</param>
        /// <param name="msg">日志内容</param>
        public void Log(EnmTdbLogLevel level, Exception exception, string msg)
        {
            //日志级别转换
            LogLevel logLevel = CvtLogLevel(level);

            this.log.Log(logLevel, exception, AddUniqueID(msg));
        }

        /// <summary>
        /// 痕迹日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Trace(string msg)
        {
            this.log.Trace(AddUniqueID(msg));
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Debug(string msg)
        {
            this.log.Debug(AddUniqueID(msg));
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Info(string msg)
        {
            this.log.Info(AddUniqueID(msg));
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Warn(string msg)
        {
            this.log.Warn(AddUniqueID(msg));
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Error(string msg)
        {
            this.log.Error(AddUniqueID(msg));
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public void Error(Exception ex, string msg)
        {
            this.log.Error(ex, AddUniqueID(msg));
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Fatal(string msg)
        {
            this.log.Fatal(AddUniqueID(msg));
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public void Fatal(Exception ex, string msg)
        {
            this.log.Fatal(ex, AddUniqueID(msg));
        }

        /// <summary>
        /// 是否启用指定级别的日志
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsEnabled(EnmTdbLogLevel level)
        {
            //日志级别转换
            LogLevel logLevel = CvtLogLevel(level);

            return this.log.IsEnabled(logLevel);
        }

        /// <summary>
        /// 是否启用Fatal级别日志
        /// </summary>
        public bool IsFatalEnabled { get { return this.log.IsFatalEnabled; } }

        /// <summary>
        /// 是否启用Error级别日志
        /// </summary>
        public bool IsErrorEnabled { get { return this.log.IsErrorEnabled; } }

        /// <summary>
        /// 是否启用Warn级别日志
        /// </summary>
        public bool IsWarnEnabled { get { return this.log.IsWarnEnabled; } }

        /// <summary>
        /// 是否启用Info级别日志
        /// </summary>
        public bool IsInfoEnabled { get { return this.log.IsInfoEnabled; } }

        /// <summary>
        /// 是否启用Debug级别日志
        /// </summary>
        public bool IsDebugEnabled { get { return this.log.IsDebugEnabled; } }

        /// <summary>
        /// 是否启用Trace级别日志
        /// </summary>
        public bool IsTraceEnabled { get { return this.log.IsTraceEnabled; } }

        #endregion

        #region 私有方法

        /// <summary>
        /// 日志级别转换
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static LogLevel CvtLogLevel(EnmTdbLogLevel level)
        {
            return level switch
            {
                EnmTdbLogLevel.Trace => LogLevel.Trace,
                EnmTdbLogLevel.Debug => LogLevel.Debug,
                EnmTdbLogLevel.Info => LogLevel.Info,
                EnmTdbLogLevel.Warn => LogLevel.Warn,
                EnmTdbLogLevel.Error => LogLevel.Error,
                EnmTdbLogLevel.Fatal => LogLevel.Fatal,
                _ => LogLevel.Off,
            };
        }

        /// <summary>
        /// 如果当前上下文是webapi请求，尝试在日志内容开头添加当前HttpContext的hash code以作唯一标识
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <returns></returns>
        private static string AddUniqueID(string msg)
        {
            //获取当前请求访问器
            var httpContextAccessor = TdbIOC.GetHttpContextAccessor();
            if (httpContextAccessor is null || httpContextAccessor.HttpContext is null)
            {
                return msg;
            }

            return $"[{httpContextAccessor.HttpContext.GetHashCode()}]{msg}";
        }

        #endregion
    }
}
