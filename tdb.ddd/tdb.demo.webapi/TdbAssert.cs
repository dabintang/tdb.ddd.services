using tdb.ddd.contracts;

namespace tdb.demo.webapi
{
    /// <summary>
    /// 断言
    /// </summary>
    public class TdbAssert
    {
        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public static void Equals<T>(T t1, T t2)
        {
            if ((t1 == null && t2 != null) || (t1 != null && t2 == null))
            {
                throw new TdbException("不相等");
            }

            if (t1 == null && t2 == null)
            {
                return;
            }

            if (t1.Equals(t2) == false)
            {
                throw new TdbException("不相等");
            }
        }
    }
}
