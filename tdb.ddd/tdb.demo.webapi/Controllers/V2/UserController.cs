using Microsoft.AspNetCore.Mvc;
using tdb.ddd.application;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.webapi;
using tdb.demo.webapi.Configs;
using tdb.demo.webapi.MockData;

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
            //找登录用户
            UserInfo? user = null;
            if (req.ID.HasValue)
            {
                user = UserRepos.Ins.GetByID(req.ID.Value);
            }
            if (user == null)
            {
                return TdbRes.Fail<V1.UserController.UserRes>(null);
            }

            var userRes = new V1.UserController.UserRes()
            {
                ID = user.ID,
                Name = user.Name,
                NickName = user.NickName
            };

            return TdbRes.Success(userRes);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        [HttpPost]
        public TdbRes<bool> UpdateUserInfo([FromBody] UpdateUserInfoReq req)
        {
            //获取用户信息
            var user = UserRepos.Ins.GetByID(req.ID);
            if (user is null)
            {
                return new TdbRes<bool>(TdbComResMsg.Fail.FromNewMsg("用户不存在"), false);
            }

            user.Name = req.Name;
            user.Pwd = req.Pwd;
            user.NickName = req.NickName ?? "";

            return TdbRes.Success(true);
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
            public long? ID { get; set; }
        }

        /// <summary>
        /// 更新用户请求条件
        /// </summary>
        public class UpdateUserInfoReq
        {
            /// <summary>
            /// 用户编号
            /// </summary>
            [TdbRequired("用户编号")]
            [TdbHashIDJsonConverter]
            public long ID { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            [TdbRequired("用户名")]
            public string Name { get; set; } = "";

            /// <summary>
            /// 密码
            /// </summary>
            [TdbRequired("密码")]
            public string Pwd { get; set; } = "";

            /// <summary>
            /// 昵称
            /// </summary>
            public string? NickName { get; set; }
        }
    }
}
