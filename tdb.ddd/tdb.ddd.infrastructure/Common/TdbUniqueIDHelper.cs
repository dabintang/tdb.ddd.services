using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// 生产唯一编码帮助类
    ///
    /// 唯一ID位数为64bit 每个bit的含义如下
    /// |1bit| 8bit  | 7bit  | 42bit |  6bit |
    /// | -  | ----- | ----- | ----- | ----- |
    /// | 2  |  256  |  128  |  4兆  |   64  |
    /// | -  | ----- | ----- | ----- | ----- |
    /// | ① |   ②  |   ③  |   ④  |   ⑤  |
    /// ①：符号位不用
    /// ②：服务ID，可以表示256个服务
    /// ③：服务序号，可以表示128个服务序号（多开负载均衡的情况）
    /// ④：时间戳，从2023-01-01（自定义的时间基准）到现在的毫秒数，可以表示139年左右
    /// ⑤：序列号，每毫秒内序列号有序，每毫秒可以产生64个序列号
    /// 最大数量  64000个/秒   64个/毫秒
    /// </summary>
    public class TdbUniqueIDHelper
    {
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object objLock = new object();

        /// <summary>
        /// 序号
        /// </summary>
        private static long seq = 0;

        /// <summary>
        /// 时间基准（单位：100纳秒）
        /// </summary>
        private static long basicTicks = new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;

        /// <summary>
        /// 最后一次生成唯一编码使用的时间部分
        /// </summary>
        private static long lastTimePart;

        /// <summary>
        /// 生成唯一编码
        /// </summary>
        /// <param name="serverID">服务ID（0-255）</param>
        /// <param name="serverSeq">服务序号（0-127）</param>
        public static long CreateID(int serverID, int serverSeq = 0)
        {
            if (serverID < 0 || serverID > 255)
            {
                throw new TdbException("服务ID值超出范围（0-255）");
            }

            if (serverSeq < 0 || serverSeq > 127)
            {
                throw new TdbException("服务序号值超出范围（0-127）");
            }

            lock (objLock)
            {
                while (true)
                {
                    //100纳秒转毫秒
                    long time = (DateTime.UtcNow.Ticks - basicTicks) / 10000;
                    if (time != lastTimePart)
                    {
                        lastTimePart = time;
                        seq = 0;
                    }

                    //序号在63以内
                    if (seq < 64)
                    {
                        break;
                    }
                    //如果毫秒内生成唯一编码超过128个，等待1毫秒后再次尝试
                    else
                    {
                        Thread.Sleep(1);
                    }
                }

                //生成唯一编码
                long uid = (serverID << 55) +
                           (serverSeq << 48) +
                           (lastTimePart << 6) +
                           seq++;
                return uid;
            }
        }
    }
}
