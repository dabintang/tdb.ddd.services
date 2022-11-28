using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.domain
{
    /// <summary>
    /// 懒加载对象
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    public abstract class TdbLazyLoadObject<T>
    {
        /// <summary>
        /// 实际值
        /// </summary>
        private T? _value;
        /// <summary>
        /// 值
        /// </summary>
        public virtual T? Value
        {
            get
            {
                if (this.IsLoaded == false)
                {
                    this.IsLoaded = true;
                    this._value = this.Load();
                }

                return this._value;
            }
            protected set
            {
                this.IsLoaded = true;
                this._value = value;
            }
        }

        /// <summary>
        /// 是否已加载
        /// （若赋过值则认为已加载，否则未加载）
        /// </summary>
        public bool IsLoaded { get; protected set; }

        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        protected abstract T? Load();
    }
}
