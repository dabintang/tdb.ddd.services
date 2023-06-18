using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.domain.contracts.Enum;
using tdb.ddd.relationships.infrastructure.Config;

namespace tdb.ddd.relationships.domain.Circle.Aggregate
{
    /// <summary>
    ///人际圈聚合
    /// </summary>
    public class CircleAgg : TdbAggregateRoot<long>
    {
        #region 仓储

        private ICircleRepos? _repos;
        /// <summary>
        /// 人际圈仓储
        /// </summary>
        private ICircleRepos Repos
        {
            get
            {
                this._repos ??= TdbIOC.GetService<ICircleRepos>();
                if (this._repos is null)
                {
                    throw new TdbException("人际圈仓储接口未实现");
                }
                return this._repos;
            }
        }

        #endregion

        #region 值

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 成员数上限
        /// </summary>
        public int MaxMembers { get; internal set; } = 200;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 创建信息
        /// </summary>
        public CreateInfoValueObject CreateInfo { get; set; } = new CreateInfoValueObject();

        /// <summary>
        /// 更新信息
        /// </summary>
        public UpdateInfoValueObject UpdateInfo { get; set; } = new UpdateInfoValueObject();

        #endregion

        #region 行为

        /// <summary>
        /// 获取圈内成员数
        /// </summary>
        /// <returns>成员数</returns>
        public async Task<int> CountMembersAsync()
        {
            return await this.Repos.CountMembersAsync(this.ID);
        }

        /// <summary>
        /// 获取成员信息
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <returns>成员信息</returns>
        public async Task<MemberEntity> GetMemberAsync(long personnelID)
        {
            return await this.Repos.GetMemberAsync(this.ID, personnelID);
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="entity">成员信息</param>
        public async Task AddMemberAsync(MemberEntity entity)
        {
            await this.Repos.SaveMemberAsync(this.ID, entity);
        }

        /// <summary>
        /// 移出成员
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        public async Task RemoveMemberAsync(long personnelID)
        {
            //获取成员信息
            var memberInfo = await this.Repos.GetMemberAsync(this.ID, personnelID);
            if (memberInfo is not null)
            {
                //删除成员
                await this.Repos.DeleteMemberAsync(memberInfo.ID);
            }
        }

        /// <summary>
        /// 移出所有成员
        /// </summary>
        public async Task RemoveAllMembersAsync()
        {
            //删除所有成员
            await this.Repos.DeleteAllMembersAsync(this.ID);
        }

        /// <summary>
        /// 设置成员角色
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <param name="roleCode">角色编码</param>
        /// <returns></returns>
        /// <exception cref="TdbException"></exception>
        public async Task<TdbRes<bool>> SetMemberRole(long personnelID, EnmRole roleCode)
        {
            //获取成员信息
            var memberInfo = await this.Repos.GetMemberAsync(this.ID, personnelID);
            if (memberInfo is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.MemberNotExist, false);
            }

            //设置圈内角色
            memberInfo.RoleCode = roleCode;
            //保存
            await this.Repos.SaveMemberAsync(this.ID, memberInfo);

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 设置成员身份
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <param name="identity">身份</param>
        public async Task<TdbRes<bool>> SetMemberIdentity(long personnelID, string identity)
        {
            //获取成员信息
            var memberInfo = await this.Repos.GetMemberAsync(this.ID, personnelID);
            if (memberInfo is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.MemberNotExist, false);
            }

            //设置圈内身份
            memberInfo.Identity = identity;
            //保存
            await this.Repos.SaveMemberAsync(this.ID, memberInfo);

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 保存人际圈信息
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await this.Repos.SaveCircleAsync(this);
        }

        /// <summary>
        /// 删除人际圈信息
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            await this.Repos.DeleteCircleAsync(this.ID);
        }

        #endregion
    }
}
