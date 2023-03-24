using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.BusMediatR;
using tdb.ddd.account.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.account.domain.User.Aggregate
{
    /// <summary>
    /// 用户角色ID懒加载
    /// </summary>
    public class UserRoleIDLazyLoad : TdbLazyLoadObject<List<long>>
    {
        #region 仓储

        private IUserRepos? _userRepos;
        /// <summary>
        /// 用户仓储
        /// </summary>
        private IUserRepos UserRepos
        {
            get
            {
                this._userRepos ??= TdbIOC.GetService<IUserRepos>();
                if (this._userRepos is null)
                {
                    throw new TdbException("用户仓储接口未实现");
                }
                return this._userRepos;
            }
        }

        #endregion

        /// <summary>
        /// 用户聚合
        /// </summary>
        private readonly UserAgg userAgg;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userAgg">用户聚合</param>
        public UserRoleIDLazyLoad(UserAgg userAgg)
        {
            this.userAgg = userAgg;
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        protected override List<long> Load()
        {
            if (this.userAgg.ID <= 0)
            {
                throw new TdbException("用户角色ID懒加载异常，用户ID不正确");
            }

            return this.UserRepos.GetRoleIDsAsync(this.userAgg.ID).Result;
        }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="lstRoleID">角色ID</param>
        internal TdbRes<bool> SetValue(List<long> lstRoleID)
        {
            //判断角色ID是否存在
            foreach (var roleID in lstRoleID)
            {
                if (TdbMediatR.SendAsync(new IsRoleExistRequest() { RoleID = roleID }).Result == false)
                {
                    return new TdbRes<bool>(AccountConfig.Msg.RoleNotExist.FromNewMsg($"角色不存在[{roleID}]"), false);
                }
            }

            //赋值
            this.Value = lstRoleID;

            return TdbRes.Success(true);
        }
    }
}
