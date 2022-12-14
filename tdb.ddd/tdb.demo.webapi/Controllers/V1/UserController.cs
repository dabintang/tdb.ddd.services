using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using tdb.ddd.application;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.webapi;
using tdb.demo.webapi.Configs;

namespace tdb.demo.webapi.Controllers.V1
{
    /// <summary>
    /// 用户
    /// </summary>
    [TdbApiVersion(1)]
    public class UserController : BaseController
    {
        /// <summary>
        /// 缓存key
        /// </summary>
        public const string CacheUsersKey = "CacheUsersKey";

        #region 接口

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns>token</returns>
        [HttpPost]
        [AllowAnonymous]
        [TdbAPILog]
        public TdbRes<string> Login([FromBody] LoginReq req)
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
            var user = lstUser.Find(m => m.Name == req.LoginName && m.Pwd == req.Password);
            if (user == null)
            {
                return new TdbRes<string>(DemoConfig.Msg.IncorrectPassword, "");
            }

            //客户端IP
            var clientIP = this.HttpContext.GetClientIP();

            //用户信息
            var lstClaim = new List<Claim>();
            lstClaim.Add(new Claim(TdbClaimTypes.UID, user.ID.ToString()));
            lstClaim.Add(new Claim(TdbClaimTypes.UName, user.Name));
            lstClaim.Add(new Claim(TdbClaimTypes.ClientIP, clientIP));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(lstClaim),
                Issuer = DemoConfig.App.Token.Issuer,
                Expires = DateTime.UtcNow.AddSeconds(DemoConfig.App.Token.TimeoutSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(DemoConfig.App.Token.SecretKey)), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return TdbRes.Success(tokenString);
        }

        /// <summary>
        /// 获取当前信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public TdbRes<UserRes> GetCurrentUserInfo()
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
            //找用户
            var user = lstUser.Find(m => m.ID == this.CurUser.ID);
            var res = new UserRes()
            {
                ID = user?.ID ?? 0,
                Name = user?.Name ?? "",
                NickName = user?.NickName ?? ""
            };

            return TdbRes.Success(res);
        }

        #endregion

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
            public string Name { get; set; }

            /// <summary>
            /// 密码
            /// </summary>           
            public string Pwd { get; set; }

            /// <summary>
            /// 昵称
            /// </summary>
            public string NickName { get; set; }
        }

        /// <summary>
        /// 登录
        /// </summary>
        public class LoginReq
        {
            /// <summary>
            /// 登录名
            /// </summary>
            [TdbRequired("登录名")]
            public string LoginName { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            [TdbRequired("密码")]
            public string Password { get; set; }
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        public class UserRes
        {
            /// <summary>
            /// 用户编号
            /// </summary>
            [TdbHashIDJsonConverter]
            public long ID { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>           
            public string Name { get; set; }

            /// <summary>
            /// 昵称
            /// </summary>
            public string NickName { get; set; }
        }
    }
}
