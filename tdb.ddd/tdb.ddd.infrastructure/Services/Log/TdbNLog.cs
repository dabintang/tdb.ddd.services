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
        /// <param name="message">日志内容</param>
        public void Log(EnmTdbLogLevel level, string message)
        {
            //日志级别转换
            LogLevel logLevel = this.CvtLogLevel(level);

            this.log.Log(logLevel, message);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="exception">异常</param>
        /// <param name="message">日志内容</param>
        public void Log(EnmTdbLogLevel level, Exception exception, string message)
        {
            //日志级别转换
            LogLevel logLevel = this.CvtLogLevel(level);

            this.log.Log(logLevel, exception, message);
        }

        /// <summary>
        /// 痕迹日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Trace(string msg)
        {
            this.log.Trace(msg);
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Debug(string msg)
        {
            this.log.Debug(msg);
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Info(string msg)
        {
            this.log.Info(msg);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Warn(string msg)
        {
            this.log.Warn(msg);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Error(string msg)
        {
            this.log.Error(msg);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public void Error(Exception ex, string msg)
        {
            this.log.Error(ex, msg);
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Fatal(string msg)
        {
            this.log.Fatal(msg);
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public void Fatal(Exception ex, string msg)
        {
            this.log.Fatal(ex, msg);
        }

        /// <summary>
        /// 是否启用指定级别的日志
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsEnabled(EnmTdbLogLevel level)
        {
            //日志级别转换
            LogLevel logLevel = this.CvtLogLevel(level);

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
        private LogLevel CvtLogLevel(EnmTdbLogLevel level)
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

        #endregion
    }
}
