using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.test.xUnit.Infrastructure
{
    /// <summary>
    /// 测试Nlog写日志
    /// </summary>
    public class TestTdbNLog
    {
        private readonly TdbNLog log = new(@"Configs\NLog.config");

        /// <summary>
        /// 输出
        /// </summary>
        private readonly ITestOutputHelper output;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="output"></param>
        public TestTdbNLog(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        [Fact]
        public void Log()
        {
            var msg = "Log";
            this.log.Log(EnmTdbLogLevel.Trace, msg);
            this.log.Log(EnmTdbLogLevel.Debug, msg);
            this.log.Log(EnmTdbLogLevel.Info, msg);
            this.log.Log(EnmTdbLogLevel.Warn, msg);
            this.log.Log(EnmTdbLogLevel.Error, msg);
            this.log.Log(EnmTdbLogLevel.Fatal, msg);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        [Fact]
        public void LogException()
        {
            var msg = "LogException";
            var ex = new Exception(msg);
            this.log.Log(EnmTdbLogLevel.Trace, ex, msg);
            this.log.Log(EnmTdbLogLevel.Debug, ex, msg);
            this.log.Log(EnmTdbLogLevel.Info, ex, msg);
            this.log.Log(EnmTdbLogLevel.Warn, ex, msg);
            this.log.Log(EnmTdbLogLevel.Error, ex, msg);
            this.log.Log(EnmTdbLogLevel.Fatal, ex, msg);
        }

        /// <summary>
        /// 痕迹日志
        /// </summary>
        [Fact]
        public void Trace()
        {
            this.log.Trace("Trace");
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        [Fact]
        public void Debug()
        {
            this.log.Trace("Debug");
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        [Fact]
        public void Info()
        {
            this.log.Info("Debug");
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        [Fact]
        public void Warn()
        {
            this.log.Warn("Warn");
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        [Fact]
        public void Error()
        {
            this.log.Error("Error");
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        [Fact]
        public void ErrorException()
        {
            var msg = "ErrorException";
            var ex = new Exception(msg);
            this.log.Error(ex, msg);
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        [Fact]
        public void Fatal()
        {
            this.log.Fatal("Fatal");
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        [Fact]
        public void FatalException()
        {
            var msg = "FatalException";
            var ex = new Exception(msg);
            this.log.Fatal(ex, msg);
        }

        /// <summary>
        /// 是否启用指定级别的日志
        /// </summary>
        [Fact]
        public void IsEnabled()
        {
            this.output.WriteLine($"Trace：{this.log.IsEnabled(EnmTdbLogLevel.Trace)}");
            this.output.WriteLine($"Debug：{this.log.IsEnabled(EnmTdbLogLevel.Debug)}");
            this.output.WriteLine($"Info：{this.log.IsEnabled(EnmTdbLogLevel.Info)}");
            this.output.WriteLine($"Warn：{this.log.IsEnabled(EnmTdbLogLevel.Warn)}");
            this.output.WriteLine($"Error：{this.log.IsEnabled(EnmTdbLogLevel.Error)}");
            this.output.WriteLine($"Fatal：{this.log.IsEnabled(EnmTdbLogLevel.Fatal)}");
            this.output.WriteLine($"Off：{this.log.IsEnabled(EnmTdbLogLevel.Off)}");
        }

    }
}
