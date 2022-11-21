using Microsoft.AspNetCore.Mvc;
using tdb.ddd.application;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.webapi;
using tdb.demo.webapi.Configs;

namespace tdb.demo.webapi.Controllers.V2
{
    /// <summary>
    /// 用户
    /// </summary>
    [TdbApiVersion(2)]
    public class UserController : BaseController
    {
        #region 接口

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        [HttpGet]
        public TdbRes<V1.UserController.UserRes> GetUserInfo([FromQuery] GetUserInfoReq req)
        {
            //用户列表
            var lstUser = TdbCache.Ins.CacheShell<List<V1.UserController.UserInfo>>(V1.UserController.CacheUsersKey, TimeSpan.FromDays(1), () =>
            {
                //用户
                var lstUser = new List<V1.UserController.UserInfo>();
                lstUser.Add(new V1.UserController.UserInfo() { ID = TdbUniqueIDHelper.CreateID(DemoConfig.App.Server.ID, DemoConfig.App.Server.Seq), Name = "a", Pwd = "a1", NickName = "<p>张三</p>" });
                lstUser.Add(new V1.UserController.UserInfo() { ID = TdbUniqueIDHelper.CreateID(DemoConfig.App.Server.ID, DemoConfig.App.Server.Seq), Name = "b", Pwd = "b2", NickName = "<p>李四</p>" });
                lstUser.Add(new V1.UserController.UserInfo() { ID = TdbUniqueIDHelper.CreateID(DemoConfig.App.Server.ID, DemoConfig.App.Server.Seq), Name = "c", Pwd = "c3", NickName = "<p>王五</p>" });
                return lstUser;
            });
            //找登录用户
            var user = lstUser.Find(m => m.ID == req.ID);
            if (user == null)
            {
                return TdbRes.Success<V1.UserController.UserRes>(null);
            }

            var userRes = new V1.UserController.UserRes()
            {
                ID = user.ID,
                Name = user.Name,
                NickName = user.NickName
            };

            return TdbRes.Success(userRes);
        }

        #endregion

        /// <summary>
        /// 获取用户信息
        /// </summary>
        public class GetUserInfoReq
        {
            /// <summary>
            /// 用户编号
            /// </summary>
            [TdbHashIDModelBinder]
            public long ID { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            public string Name { get; set; }
        }
    }
}
