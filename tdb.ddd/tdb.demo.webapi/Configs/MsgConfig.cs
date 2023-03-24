using tdb.ddd.contracts;

namespace tdb.demo.webapi.Configs
{
    /// <summary>
    /// 回报消息配置
    /// </summary>
    public class MsgConfig
    {
        /// <summary>
        /// 登录名或密码不对
        /// </summary>
        public TdbResMsgInfo? IncorrectPassword;//{ get; set; }
    }
}
