using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.domain;
using tdb.ddd.webapi;

namespace tdb.demo.webapi.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    [ApiController]
    [Authorize]
    [TdbClientIPAuth()]
    [Route("tdb/demo/v{api-version:apiVersion}/[controller]/[action]")]
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
                }

                return this._curUser;
            }
        }
    }

    /// <summary>
    /// 操作人信息
    /// </summary>
    public class OperatorInfo
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
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
    }
}
