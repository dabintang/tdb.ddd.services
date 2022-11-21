using System.ComponentModel.DataAnnotations;
using tdb.common;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 必须验证（不能为null或空字符串）
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TdbRequiredAttribute : RequiredAttribute
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paramName">参数名</param>
        public TdbRequiredAttribute(string paramName)
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
            //参数错误信息
            var errInfo = new TdbInvalidParamInfo(this.GetType(), $"{this.ParamName}不能为空");
            return errInfo.SerializeJson();
        }
    }
}
