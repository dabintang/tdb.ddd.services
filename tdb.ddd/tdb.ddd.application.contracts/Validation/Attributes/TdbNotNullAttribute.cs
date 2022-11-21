using System.ComponentModel.DataAnnotations;
using tdb.common;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 不能为null
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TdbNotNullAttribute : ValidationAttribute
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paramName">参数名</param>
        public TdbNotNullAttribute(string paramName)
        {
            this.ParamName = paramName;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            return (value != null);
        }

        /// <summary>
        /// 格式化消息字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            //参数错误信息
            var errInfo = new TdbInvalidParamInfo(this.GetType(), $"{this.ParamName}不能为null");
            return errInfo.SerializeJson();
        }
    }
}
