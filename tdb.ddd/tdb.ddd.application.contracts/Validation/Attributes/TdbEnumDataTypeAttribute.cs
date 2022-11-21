using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 枚举类型合法性验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TdbEnumDataTypeAttribute : ValidationAttribute
    {
        /// <summary>
        /// 错误提示
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 枚举类型
        /// </summary>
        public Type EnumType { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="errMsg">错误提示</param>
        public TdbEnumDataTypeAttribute(Type enumType, string errMsg)
        {
            this.EnumType = enumType;
            this.ErrMsg = errMsg;
        }

        /// <summary>
        /// 验证合法性
        /// </summary>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            return Enum.IsDefined(this.EnumType, value);
        }

        /// <summary>
        /// 格式化消息字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            //参数错误信息
            var errInfo = new TdbInvalidParamInfo(this.GetType(), this.ErrMsg);
            return errInfo.SerializeJson();
        }
    }
}
