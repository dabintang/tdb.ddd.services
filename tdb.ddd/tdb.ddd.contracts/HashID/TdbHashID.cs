using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 对ID进行编码或解码
    /// </summary>
    public class TdbHashID
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="minHashLength">最小长度</param>
        /// <param name="alphabet">hash字母表</param>
        /// <param name="seps"></param>
        public static void Init(string salt, int minHashLength = 6, string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", string seps = "cfhistuCFHISTU")
        {
            hashids = new(salt, minHashLength, alphabet, seps);
        }

        /// <summary>
        /// Hashids
        /// </summary>
        private static HashIDService? hashids;

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeLong(long value)
        {
            if (hashids == null)
            {
                throw new TdbException("请先初始化[TdbHashID.Init]");
            }

            return hashids.EncodeLong(value);
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long DecodeSingleLong(string value)
        {
            if (hashids == null)
            {
                throw new TdbException("请先初始化[TdbHashID.Init]");
            }

            return hashids.DecodeSingleLong(value);
        }
    }
}
