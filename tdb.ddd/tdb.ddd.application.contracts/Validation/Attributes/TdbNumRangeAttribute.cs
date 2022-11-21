using System.ComponentModel.DataAnnotations;
using tdb.common;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 数值范围验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TdbNumRangeAttribute : ValidationAttribute
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue { get; set; } = double.MinValue;

        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue { get; set; } = double.MaxValue;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paramName">参数名</param>
        public TdbNumRangeAttribute(string paramName)
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
            var dVal = Convert.ToDouble(value);

            if (dVal < this.MinValue || dVal > this.MaxValue)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 格式化消息字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            string msg;
            if (this.MinValue != double.MinValue && this.MaxValue != double.MaxValue)
            {
                msg = $"{this.ParamName}的值应该在{this.MinValue}-{this.MaxValue}范围内";
            }
            else if (this.MinValue != double.MinValue)
            {
                msg = $"{ParamName}的值应该大于{this.MinValue}";
            }
            else if (this.MaxValue != double.MaxValue)
            {
                msg = $"{ParamName}的值应该小于{this.MaxValue}";
            }
            else
            {
                msg = $"{this.ParamName}的值应该在{this.MinValue}-{this.MaxValue}范围内";
            }

            //参数错误信息
            var errInfo = new TdbInvalidParamInfo(this.GetType(), msg);
            return errInfo.SerializeJson();
        }
    }
}
