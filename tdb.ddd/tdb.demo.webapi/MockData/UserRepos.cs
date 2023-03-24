using System.Xml.Linq;
using tdb.ddd.infrastructure;
using tdb.demo.webapi.Configs;

namespace tdb.demo.webapi.MockData
{
    /// <summary>
    /// 用户信息仓储
    /// </summary>
    public class UserRepos
    {
        #region 单例

        /// <summary>
        /// 实例
        /// </summary>
        public static UserRepos Ins { get; set; } = new UserRepos();

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private UserRepos()
        {
            //用户列表初始化
            this.LstUser = new List<UserInfo>();
            this.LstUser.Add(new UserInfo() { ID = 1, Name = "a", Pwd = "a1", NickName = "<p>张三</p>" });
            this.LstUser.Add(new UserInfo() { ID = 2, Name = "b", Pwd = "b2", NickName = "<p>李四</p>" });
            this.LstUser.Add(new UserInfo() { ID = 3, Name = "c", Pwd = "c3", NickName = "<p>王五</p>" });
        }

        #endregion

        /// <summary>
        /// 用户列表
        /// </summary>
        private List<UserInfo> LstUser { get; set; }

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public UserInfo? GetByID(long id)
        {
            var user = this.LstUser.Find(m => m.ID == id);
            return user;
        }

        /// <summary>
        /// 通知用户名和密码查找用户
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public UserInfo? Find(string name, string pwd)
        {
            var user = this.LstUser.Find(m => m.Name == name && m.Pwd == pwd);
            return user;
        }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>           
        public string Name { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>           
        public string Pwd { get; set; } = "";

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; } = "";
    }

}
