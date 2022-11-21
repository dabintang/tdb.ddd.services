using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using tdb.common;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 正则表达式验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TdbRegexAttribute : ValidationAttribute
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegexText { get; set; }

        /// <summary>
        /// 错误提示
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="regexText">正则表达式</param>
        /// <param name="errMsg">错误提示</param>
        public TdbRegexAttribute(string regexText, string errMsg)
        {
            this.RegexText = regexText;
            this.ErrMsg = errMsg;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            var reg = new Regex(this.RegexText);
            return reg.IsMatch(Convert.ToString(value) ?? "");
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
