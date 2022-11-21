﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.Authority.Aggregate;
using tdb.account.domain.Role;
using tdb.account.domain.Role.Aggregate;
using tdb.account.domain.User;
using tdb.account.domain.User.Aggregate;
using tdb.account.repository.DBEntity;
using tdb.common;
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;
using static tdb.account.domain.contracts.Const.Cst;

namespace tdb.account.repository
{
    /// <summary>
    /// 用户仓储
    /// </summary>
    public class UserRepos : TdbRepository<UserInfo>, IUserRepos
    {
        /// <summary>
        /// 聚合备份
        /// （key：权限ID；value：聚合）
        /// </summary>
        private readonly Dictionary<long, UserAgg> dicBackup = new();

        #region 实现接口

        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public async Task<UserAgg> GetUserAggAsync(long userID)
        {
            //获取用户信息
            var userInfo = await TdbCache.Ins.HCacheShellAsync(Cst_CacheKeyUserInfo, TimeSpan.FromDays(1), userID.ToStr(), async () => await this.GetFirstAsync(m => m.ID == userID && m.IsDeleted == false));
            if (userInfo == null)
            {
                return null;
            }

            //转换为聚合结构
            var userAgg = DBMapper.Map<UserInfo, UserAgg>(userInfo);
            //备份
            dicBackup[userAgg.ID] = userAgg;

            return userAgg;
        }

        /// <summary>
        /// 获取用户聚合
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="password">密码（MD5）</param>
        /// <returns></returns>
        public async Task<UserAgg> GetUserAggAsync(string loginName, string password)
        {
            TdbLogger.Ins.Debug($"UserRepos ContextID：{this.Context.ContextID}");

            //获取用户信息
            var userInfo = await this.GetFirstAsync(m => m.LoginName == loginName && m.Password == password && m.IsDeleted == false);
            if (userInfo == null)
            {
                return null;
            }

            //缓存
            await TdbCache.Ins.HSetAsync(Cst_CacheKeyUserInfo, TimeSpan.FromDays(1), userInfo.ID.ToStr(), userInfo);

            //转换为聚合结构
            var userAgg = DBMapper.Map<UserInfo, UserAgg>(userInfo);

            return userAgg;
        }

        /// <summary>
        /// 获取用户拥有的角色ID
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public async Task<List<int>> GetRoleIDsAsync(long userID)
        {
            return await TdbCache.Ins.CacheShellAsync<List<int>>(CacheKeyUserRoleID(userID), TimeSpan.FromDays(1), async () =>
            {
                return await this.Change<UserRoleConfig>().AsQueryable().Where(m => m.UserID == userID).Select(m => m.RoleID).ToListAsync();
            });
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="agg">用户聚合</param>
        public async Task SaveChangedAsync(UserAgg agg)
        {
            //获取备份
            this.dicBackup.TryGetValue(agg.ID, out UserAgg aggBackup);

            //用户信息是否有改动
            if (agg.Equals(aggBackup) == false)
            {
                //转换为数据库实体
                var info = DBMapper.Map<UserAgg, UserInfo>(agg);

                //如果有备份，则说明是更新操作
                if (aggBackup != null)
                {
                    await this.AsUpdateable(info).ExecuteCommandAsync();
                }
                else
                {
                    await this.InsertOrUpdateAsync(info);
                }
                //清缓存
                TdbCache.Ins.HDel(Cst_CacheKeyUserInfo, agg.ID.ToStr());

                dicBackup[agg.ID] = agg;
            }

            //用户角色是否有改动
            if (agg.LstRoleID.IsLoaded)
            {
                //转换为数据库实体
                var lstRole = ToUserRoleConfig(agg);
                //保存用户角色信息
                await this.SaveUserRoleConfig(agg.ID, lstRole);
            }
        }

        /// <summary>
        /// 添加登录记录
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="clientIP">登录端IP</param>
        /// <param name="loginTime">登录时间</param>
        public async Task AddLoginRecordAsync(long userID, string clientIP, DateTime loginTime)
        {
            //登录记录
            var loginRecord = new LoginRecord()
            {
                UserID = userID,
                ClientIP = clientIP,
                LoginTime = loginTime
            };

            TdbLogger.Ins.Debug($"UserRepos ContextID：{this.Context.ContextID}");

            await this.Change<LoginRecord>().AsInsertable(loginRecord).ExecuteCommandAsync();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 保存用户角色信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="lstRole">用户角色</param>
        /// <returns></returns>
        private async Task SaveUserRoleConfig(long userID, List<UserRoleConfig> lstRole)
        {
            var client = this.Change<UserRoleConfig>();

            //先删除原角色
            await client.AsDeleteable().Where(m => m.UserID == userID).ExecuteCommandAsync();
            //清缓存
            TdbCache.Ins.Del(CacheKeyUserRoleID(userID));

            //保存新角色
            if (lstRole != null && lstRole.Count > 0)
            {
                await client.AsInsertable(lstRole).ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 转换为数据库实体
        /// </summary>
        /// <param name="agg">聚合</param>
        /// <returns></returns>
        private static List<UserRoleConfig> ToUserRoleConfig(UserAgg agg)
        {
            var list = new List<UserRoleConfig>();
            foreach (var roleID in agg.LstRoleID.Value)
            {
                list.Add(new UserRoleConfig() { UserID = agg.ID, RoleID = roleID });
            }
            return list;
        }

        #endregion

        #region 缓存key

        /// <summary>
        /// 【用户信息】缓存key
        /// </summary>
        private const string Cst_CacheKeyUserInfo = "ReposUserInfo";

        /// <summary>
        /// 获取【用户角色ID】缓存key
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        private static string CacheKeyUserRoleID(long userID)
        {
            return $"ReposUserRoleID_{userID}";
        }

        #endregion
    }
}
