using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.account.application.contracts;
using tdb.common;
using tdb.ddd.domain;
using tdb.ddd.webapi;

namespace tdb.account.webapi.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    [ApiController]
    [Authorize]
    [TdbClientIPAuth()]
    [Route("tdb/account/v{api-version:apiVersion}/[controller]/[action]")]
    public class BaseController : ControllerBase
    {
        private OperatorInfo _curUser;
        /// <summary>
        /// 当前用户
        /// </summary>
        protected virtual OperatorInfo CurUser
        {
            get
            {
                //无认证用户
                if (HttpContext.User == null)
                {
                    return null;
                }

                if (this._curUser == null)
                {
                    this._curUser = new OperatorInfo();
                    this._curUser.ID = Convert.ToInt64(HttpContext.User.FindFirst(TdbClaimTypes.UID).Value);
                    this._curUser.Name = HttpContext.User.Identity.Name;

                    //角色
                    var roleClaims = HttpContext.User.FindAll(TdbClaimTypes.RoleID);
                    this._curUser.LstRoleID = roleClaims.SelectMany(m => m.Value.DeserializeJson<List<int>>()).ToList();

                    //权限
                    var authorityClaims = HttpContext.User.FindAll(TdbClaimTypes.AuthorityID);
                    this._curUser.LstAuthorityID = authorityClaims.SelectMany(m => m.Value.DeserializeJson<List<int>>()).ToList();
                }

                return this._curUser;
            }
        }
    }
}
