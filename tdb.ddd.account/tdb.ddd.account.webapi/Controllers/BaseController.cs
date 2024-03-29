﻿using Microsoft.AspNetCore.Mvc;
using tdb.ddd.account.application.contracts;
using tdb.common;
using tdb.ddd.domain;
using tdb.ddd.webapi;
using tdb.ddd.application.contracts;
using Microsoft.AspNetCore.Authorization;
using Autofac.Core;
using Microsoft.Extensions.Options;
using System.Threading;

namespace tdb.ddd.account.webapi.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    [ApiController]
    [Authorize]
    [TdbAuthClientIP()]
    [Route("tdb.ddd.account/v{api-version:apiVersion}/[controller]/[action]")]
    public class BaseController : ControllerBase
    {
        private OperatorInfo? _curUser;
        /// <summary>
        /// 当前用户
        /// </summary>
        protected virtual OperatorInfo? CurUser
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
                    this._curUser = new OperatorInfo
                    {
                        ID = Convert.ToInt64(HttpContext.User.FindFirst(TdbClaimTypes.UID)?.Value),
                        Name = HttpContext.User.Identity?.Name ?? ""
                    };

                    //角色
                    var roleClaims = HttpContext.User.FindAll(TdbClaimTypes.RoleID);
                    this._curUser.LstRoleID = roleClaims.SelectMany(m => m.Value.DeserializeJson<List<long>>() ?? new List<long>()).ToList();

                    //权限
                    var authorityClaims = HttpContext.User.FindAll(TdbClaimTypes.AuthorityID);
                    this._curUser.LstAuthorityID = authorityClaims.SelectMany(m => m.Value.DeserializeJson<List<long>>() ?? new List<long>()).ToList();
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
            var reqOpe = new TdbOperateReq<T>(param, this.CurUser?.ID ?? 0, this.CurUser?.Name ?? "")
            {
                OperatorRoleIDs = this.CurUser?.LstRoleID ?? new List<long>(),
                OperatorAuthorityIDs = this.CurUser?.LstAuthorityID ?? new List<long>()
            };

            return reqOpe;
        }
    }
}
