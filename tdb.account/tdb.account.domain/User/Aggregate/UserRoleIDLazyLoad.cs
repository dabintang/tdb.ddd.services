using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.BusMediatR;
using tdb.account.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

namespace tdb.account.domain.User.Aggregate
{
    /// <summary>
    /// 用户角色ID懒加载
    /// </summary>
    public class UserRoleIDLazyLoad : TdbLazyLoadObject<List<int>>
    {
        #region 仓储

        private IUserRepos _userRepos;
        /// <summary>
        /// 用户仓储
        /// </summary>
        private IUserRepos UserRepos
        {
            get
            {
                this._userRepos ??= TdbIOC.GetService<IUserRepos>();
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
        protected override List<int> Load()
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
        internal TdbRes<bool> SetValue(List<int> lstRoleID)
        {
            //判断角色ID是否存在
            foreach (var roleID in lstRoleID)
            {
                if (TdbMediatR.Send(new IsRoleExistRequest() { RoleID = roleID }).Result == false)
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
