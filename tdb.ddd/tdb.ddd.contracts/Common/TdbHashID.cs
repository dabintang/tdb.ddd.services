using HashidsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 对ID进行编码或解码
    /// </summary>
    public class TdbHashID
    {
        /// <summary>
        /// Hashids
        /// </summary>
        private readonly static Hashids hashids = new("tangdabinok", 6);//加盐

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeLong(long value)
        {
            return hashids.EncodeLong(value);
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long DecodeSingleLong(string value)
        {
            return hashids.DecodeSingleLong(value);
        }
    }
}
