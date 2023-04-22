using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 返回结果包装器
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public class TdbRes<T>
    {
        /// <summary>
        /// 编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; } = "";

        /// <summary>
        /// 结果
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msgInfo">消息</param>
        /// <param name="data">结果</param>
        public TdbRes(TdbResMsgInfo msgInfo, T? data)
        {
            this.Code = msgInfo.Code;
            this.Msg = msgInfo.Msg;
            this.Data = data;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TdbRes()
        {
        }
    }

    /// <summary>
    /// 返回结果包装器
    /// </summary>
    public class TdbRes
    {
        /// <summary>
        /// 成功消息
        /// </summary>
        /// <param name="data">返回值</param>
        /// <returns>成功消息</returns>
        public static TdbRes<T> Success<T>(T? data)
        {
            return new TdbRes<T>(TdbComResMsg.Success, data);
        }

        /// <summary>
        /// 失败消息
        /// </summary>
        /// <param name="data">返回值</param>
        /// <returns>失败消息</returns>
        public static TdbRes<T> Fail<T>(T? data = default)
        {
            return new TdbRes<T>(TdbComResMsg.Fail, data);
        }
    }
}
