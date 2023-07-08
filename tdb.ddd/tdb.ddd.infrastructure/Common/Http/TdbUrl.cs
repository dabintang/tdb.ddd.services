using Castle.Core.Internal;
using Flurl;
using Flurl.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.infrastructure
{
    /// <summary>
    /// Url对象
    /// </summary>
    public class TdbUrl
    {
        /// <summary>
        /// Flurl.Url
        /// </summary>
        protected Url flurl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">url字符串</param>
        public TdbUrl(string url)
        {
            this.flurl = Url.Parse(url);
        }

        #region 公开方法

        /// <summary>
        /// 设置查询参数，如果参数已存在则覆盖原参数
        /// </summary>
        /// <param name="values">参数对象</param>
        /// <param name="nullValueHandling">指示如何处理空值，默认为删除</param>
        /// <returns>添加了查询参数的Url对象</returns>
        public TdbUrl SetQueryParams(object? values, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            if (values == null)
            {
                return this;
            }

            if (values is string s)
            {
                this.flurl.SetQueryParam(s);
                return this;
            }

            foreach (var (key, value) in ToKeyValuePairs(values))
            {
                this.flurl.SetQueryParam(key, value, nullValueHandling);
            }
            return this;
        }

        /// <summary>
        /// 转为uri
        /// </summary>
        /// <returns></returns>
        public Uri ToUri()
        {
            return this.flurl.ToUri();
        }

        /// <summary>
        /// （对[Flurl.Util.CommonExtensions.ToKeyValuePairs]改造，添加hashid处理）
        /// Returns a key-value-pairs representation of the object.
        /// For strings, URL query string format assumed and pairs are parsed from that.
        /// For objects that already implement IEnumerable&lt;KeyValuePair&gt;, the object itself is simply returned.
        /// For all other objects, all publicly readable properties are extracted and returned as pairs.
        /// </summary>
        /// <param name="obj">The object to parse into key-value pairs</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null" />.</exception>
        public static IEnumerable<(string Key, object? Value)> ToKeyValuePairs(object? obj)
        {
            if (obj is null)
            {
                return Enumerable.Empty<(string, object?)>();
            }

            return
                obj is string s ? StringToKV(s) :
                obj is IEnumerable e ? CollectionToKV(e) :
                ObjectToKV(obj);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 字符串转key-value对
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static IEnumerable<(string Key, object? Value)> StringToKV(string s)
        {
            if (string.IsNullOrEmpty(s))
                return Enumerable.Empty<(string, object?)>();

            return
                from p in s.Split('&')
                let pair = p.SplitOnFirstOccurence("=")
                let name = pair[0]
                let value = (pair.Length == 1) ? null : pair[1]
                select (name, (object)value);
        }

        /// <summary>
        /// 集合转key-value对
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private static IEnumerable<(string Key, object? Value)> CollectionToKV(IEnumerable col)
        {
            foreach (var item in col)
            {
                if (item == null)
                    continue;
                if (!IsTuple2(item, out var name, out var val) && !LooksLikeKV(item, out name, out val))
                    yield return (CommonExtensions.ToInvariantString(item), null);
                else if (name != null)
                    yield return (CommonExtensions.ToInvariantString(item), val);
            }
        }

        /// <summary>
        /// 对象实例转key-value对
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns></returns>
        private static IEnumerable<(string Name, object? Value)> ObjectToKV(object obj)
        {
            var lstKeyValue = new List<(string Name, object? Value)>();
            if (obj is null)
            {
                return lstKeyValue;
            }

            //属性
            foreach (var prop in obj.GetType().GetProperties())
            {
                object? value = prop.GetValue(obj);
                if (value is IEnumerable e)
                {
                    lstKeyValue.AddRange(CollectionToKV(e));
                }
                else
                {
                    value = GetDeclaredTypeValue(value, prop.PropertyType);
                    if (value is long longVal && prop.GetAttribute<TdbHashIDJsonConverterAttribute>() is not null)
                    {
                        value = TdbHashID.EncodeLong(longVal);
                    }
                    lstKeyValue.Add((prop.Name, value));
                }
            }

            //字段
            foreach (var field in obj.GetType().GetFields())
            {
                object? value = field.GetValue(obj);
                if (value is IEnumerable e)
                {
                    lstKeyValue.AddRange(CollectionToKV(e));
                }
                else
                {
                    value = GetDeclaredTypeValue(value, field.FieldType);
                    if (value is long longVal && field.GetAttribute<TdbHashIDJsonConverterAttribute>() is not null)
                    {
                        value = TdbHashID.EncodeLong(longVal);
                    }
                    lstKeyValue.Add((field.Name, value));
                }
            }

            return lstKeyValue;
        }

        /// <summary>
        /// 尝试获取属性/字段值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool TryGetPropOrField(object obj, string name, out object? value)
        {
            //属性
            var prop = obj.GetType().GetProperty(name);
            if (prop != null)
            {
                value = prop.GetValue(obj, null);
                value = GetDeclaredTypeValue(value, prop.PropertyType);
                if (value is long longVal && prop.GetAttribute<TdbHashIDJsonConverterAttribute>() is not null)
                {
                    value = TdbHashID.EncodeLong(longVal);
                }
                return true;
            }

            //字段
            var field = obj.GetType().GetField(name);
            if (field != null)
            {
                value = field.GetValue(obj);
                value = GetDeclaredTypeValue(value, field.FieldType);
                if (value is long longVal && field.GetAttribute<TdbHashIDJsonConverterAttribute>() is not null)
                {
                    value = TdbHashID.EncodeLong(longVal);
                }
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private static bool IsTuple2(object item, out object? name, out object? val)
        {
            name = null;
            val = null;
            return
                OrdinalContains(item.GetType().Name, "Tuple") &&
                TryGetPropOrField(item, "Item1", out name) &&
                TryGetPropOrField(item, "Item2", out val) &&
                !TryGetPropOrField(item, "Item3", out _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private static bool LooksLikeKV(object item, out object? name, out object? val)
        {
            //name = null;
            val = null;
            return
                (TryGetPropOrField(item, "Key", out name) || TryGetPropOrField(item, "key", out name) || TryGetPropOrField(item, "Name", out name) || TryGetPropOrField(item, "name", out name)) &&
                (TryGetPropOrField(item, "Value", out val) || TryGetPropOrField(item, "value", out val));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        private static bool OrdinalContains(string s, string value, bool ignoreCase = false) =>
            s != null && s.Contains(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="declaredType"></param>
        /// <returns></returns>
        private static object? GetDeclaredTypeValue(object? value, Type declaredType)
        {
            if (value == null || value.GetType() == declaredType)
                return value;

            // without this we had https://github.com/tmenier/Flurl/issues/669
            // related: https://stackoverflow.com/q/3531318/62600
            declaredType = Nullable.GetUnderlyingType(declaredType) ?? declaredType;

            // added to deal with https://github.com/tmenier/Flurl/issues/632
            // thx @j2jensen!
            if (value is IEnumerable col
                && declaredType.IsGenericType
                && declaredType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && !col.GetType().GetInterfaces().Contains(declaredType)
                && declaredType.IsInstanceOfType(col))
            {
                var elementType = declaredType.GetGenericArguments()[0];
                return col.Cast<object>().Select(element => Convert.ChangeType(element, elementType));
            }

            return value;
        }

        #endregion
    }
}
