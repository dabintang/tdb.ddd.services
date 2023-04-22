using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.webapi;
using tdb.demo.webapi.Configs;
using tdb.demo.webapi.MockData;

namespace tdb.demo.webapi.Controllers.V1
{
    /// <summary>
    /// 用户
    /// </summary>
    [TdbApiVersion(1)]
    public class UserController : BaseController
    {
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
            //找登录用户
            var user = UserRepos.Ins.Find(req.LoginName, req.Password);
            if (user == null)
            {
                return new TdbRes<string>(DemoConfig.Msg!.IncorrectPassword!, "");
            }

            //客户端IP
            var clientIP = this.HttpContext.GetClientIP();

            //用户信息
            var lstClaim = new List<Claim>
            {
                new Claim(TdbClaimTypes.UID, user.ID.ToString()),
                new Claim(TdbClaimTypes.UName, user.Name),
                new Claim(TdbClaimTypes.ClientIP, clientIP)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(lstClaim),
                Issuer = DemoConfig.App!.Token!.Issuer,
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
        [TdbAuthWhiteListIP]
        public TdbRes<UserRes> GetCurrentUserInfo()
        {
            //找用户
            var user = UserRepos.Ins.GetByID(this.CurUser?.ID ?? 0);
            var res = new UserRes()
            {
                ID = user?.ID ?? 0,
                Name = user?.Name ?? "",
                NickName = user?.NickName ?? ""
            };

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        //[HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public TdbPageRes<UserRes> QueryUserList([FromBody]QueryUserListReq req)
        {
            var all = UserRepos.Ins.All();
            var enumerable = all.AsEnumerable();

            if (req.ID is not null)
            {
                enumerable = enumerable.Where(m => m.ID == req.ID.Value);
            }
            else if (req.LstID?.Count > 0)
            {
                enumerable = enumerable.Where(m => req.LstID.Contains(m.ID));
            }

            //排序
            if (req.LstSortItem is not null)
            {
                foreach (var sort in req.LstSortItem)
                {
                    switch (sort.FieldCode)
                    {
                        case QueryUserListReq.EnmSortField.ID:
                            enumerable = enumerable.Sort(sort.SortCode, m => m.ID);
                            break;
                        case QueryUserListReq.EnmSortField.Name:
                            enumerable = enumerable.Sort(sort.SortCode, m => m.Name);
                            break;
                        case QueryUserListReq.EnmSortField.NickName:
                            enumerable = enumerable.Sort(sort.SortCode, m => m.NickName);
                            break;
                    }
                }
            }
            //分页
            var list = enumerable.Skip(Math.Min((req.PageNO - 1), 1) * req.PageSize).Take(req.PageSize).Select(m => new UserRes()
            {
                ID = m.ID,
                Name = m.Name,
                NickName = m.NickName
            }).ToList();
            return new TdbPageRes<UserRes>(TdbComResMsg.Success, list, all.Count);
        }

        #endregion

        /// <summary>
        /// 登录
        /// </summary>
        public class LoginReq
        {
            /// <summary>
            /// 登录名
            /// </summary>
            [TdbRequired("登录名")]
            public string LoginName { get; set; } = "";

            /// <summary>
            /// 密码
            /// </summary>
            [TdbRequired("密码")]
            public string Password { get; set; } = "";
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
            public string Name { get; set; } = "";

            /// <summary>
            /// 昵称
            /// </summary>
            public string NickName { get; set; } = "";
        }

        /// <summary>
        /// 查询用户列表 请求参数
        /// </summary>
        public class QueryUserListReq : TdbPageReqBase<QueryUserListReq.EnmSortField>
        {
            /// <summary>
            /// [可选]用户编号集合
            /// </summary>
            [TdbHashIDListJsonConverter]
            public List<long>? LstID { get; set; }

            /// <summary>
            /// [可选]用户编号
            /// </summary>
            [TdbHashIDJsonConverter]
            public long? ID { get; set; }

            #region 内部类

            /// <summary>
            /// 排序字段枚举（1：用户编号；2：用户名；3：昵称）
            /// </summary>
            public enum EnmSortField
            {
                /// <summary>
                /// 用户编号
                /// </summary>
                ID = 1,

                /// <summary>
                /// 用户名
                /// </summary>
                Name = 2,

                /// <summary>
                /// 昵称
                /// </summary>
                NickName = 3
            }

            #endregion
        }
    }
}
