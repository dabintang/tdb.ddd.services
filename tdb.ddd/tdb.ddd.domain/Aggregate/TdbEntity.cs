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
    /// 实体
    /// </summary>
    /// <typeparam name="TKey">主键ID类型</typeparam>
    public abstract class TdbEntity<TKey>
    { 
        /// <summary>
        /// 主键
        /// </summary>
        public virtual TKey? ID { get; set; } = default;

        /// <summary>
        /// 比较（懒加载值不做比较）
        /// </summary>
        /// <param name="obj">用于比较的对象</param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj is not TdbEntity<TKey>)
            {
                return false;
            }

            //是否同一实例
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var equalEntity = (TdbEntity<TKey>)obj;

            //如果2个实例的ID都是默认值，认为不相等
            if (EqualityComparer<TKey>.Default.Equals(this.ID, default) && EqualityComparer<TKey>.Default.Equals(equalEntity.ID, default))
            {
                return false;
            }

            //比较类型
            var typeOfThis = GetType().GetTypeInfo();
            var typeOfOther = equalEntity.GetType().GetTypeInfo();
            if (!typeOfThis.IsAssignableFrom(typeOfOther) && !typeOfOther.IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return GetEqualityItems().SequenceEqual(equalEntity.GetEqualityItems());
            //return this.ID?.Equals(equalEntity.ID) ?? false;
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (this.ID == null)
            {
                return 0;
            }

            return this.ID.GetHashCode();
        }

        /// <summary>
        /// 获取用于比较的属性或字段
        /// （剔除懒加载值）
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<object?> GetEqualityItems()
        {
            var properties = this.GetType().GetProperties();
            foreach (var property in properties)
            {
                //剔除懒加载值
                if (CheckHelper.IsSubclassOf(property.PropertyType, typeof(TdbLazyLoadObject<>)))
                {
                    continue;
                }

                yield return property.GetValue(this);
            }

            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                //剔除懒加载值
                if (CheckHelper.IsSubclassOf(field.FieldType, typeof(TdbLazyLoadObject<>)))
                {
                    continue;
                }

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
        public static bool operator ==(TdbEntity<TKey> left, TdbEntity<TKey> right)
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
        public static bool operator !=(TdbEntity<TKey> left, TdbEntity<TKey> right)
        {
            return !(left == right);
        }

        #endregion
    }
}
