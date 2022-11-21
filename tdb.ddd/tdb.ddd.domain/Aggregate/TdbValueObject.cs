using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.ddd.domain
{
    /// <summary>
    /// 值对象
    /// </summary>
    public abstract class TdbValueObject
    {
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj">用于比较的对象</param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            var other = (TdbValueObject)obj;
            return GetEqualityItems().SequenceEqual(other.GetEqualityItems());
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return GetEqualityItems()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// 获取用于比较的属性或字段
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<object?> GetEqualityItems()
        {
            var properties = this.GetType().GetProperties();
            foreach (var property in properties)
            {
                yield return property.GetValue(this);
            }

            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                yield return field.GetValue(this);
            }
        }

        #region 重写运算符

        /// <summary>
        /// 相等运算符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(TdbValueObject left, TdbValueObject right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        /// <summary>
        /// 不等运算符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(TdbValueObject left, TdbValueObject right)
        {
            return !(left == right);
        }

        #endregion
    }
}
