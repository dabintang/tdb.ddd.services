using tdb.ddd.infrastructure.Services;

namespace tdb.account.tools.Configs
{
    /// <summary>
    /// appsettings.json配置
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        [TdbConfigKey("DBConnStr")]
        public string DBConnStr { get; set; }

        ///// <summary>
        ///// 超级管理员默认密码
        ///// </summary>
        //[TdbConfigKey("SuperAdminDefaultPwd")]
        //public string SuperAdminDefaultPwd { get; set; }
    }
}
