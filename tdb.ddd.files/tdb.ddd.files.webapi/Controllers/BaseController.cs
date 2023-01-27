using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tdb.common;
using tdb.ddd.application.contracts;
using tdb.ddd.domain;
using tdb.ddd.files.application.contracts;
using tdb.ddd.webapi;

namespace tdb.ddd.files.webapi.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    [ApiController]
    [Authorize]
    [TdbAuthClientIP()]
    [Route("tdb.ddd.files/v{api-version:apiVersion}/[controller]/[action]")]
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
                    this._curUser.LstRoleID = roleClaims.SelectMany(m => m.Value.DeserializeJson<List<long>>()).ToList();

                    //权限
                    var authorityClaims = HttpContext.User.FindAll(TdbClaimTypes.AuthorityID);
                    this._curUser.LstAuthorityID = authorityClaims.SelectMany(m => m.Value.DeserializeJson<List<long>>()).ToList();
                }

                return this._curUser;
            }
        }

        /// <summary>
        /// 生成操作请求参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param">请求参数</param>
        /// <returns></returns>
        protected virtual TdbOperateReq<T> CreateTdbOperateReq<T>(T param)
        {
            var reqOpe = new TdbOperateReq<T>()
            {
                OperatorID = this.CurUser.ID,
                OperatorName = this.CurUser.Name,
                OperatorRoleIDs = this.CurUser.LstRoleID,
                OperatorAuthorityIDs = this.CurUser.LstAuthorityID,
                Param = param
            };

            return reqOpe;
        }
    }
}
