using System.ComponentModel.DataAnnotations;
using tdb.common;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 字符串长度验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TdbStringLengthAttribute : StringLengthAttribute
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="maximumLength">最大长度</param>
        public TdbStringLengthAttribute(string paramName, int maximumLength) : base(maximumLength)
        {
            this.ParamName = paramName;
        }

        /// <summary>
        /// 格式化消息字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            string msg;
            if (this.MinimumLength > 0)
            {
                msg = $"{ParamName}的长度应该在{this.MinimumLength}-{this.MaximumLength}个字符范围内";
            }
            else
            {
                msg = $"{ParamName}的长度不能超过{this.MaximumLength}个字符";
            }

            //参数错误信息
            var errInfo = new TdbInvalidParamInfo(this.GetType(), msg);
            return errInfo.SerializeJson();
        }
    }
}
