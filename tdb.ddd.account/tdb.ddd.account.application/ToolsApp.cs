using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Authority;
using tdb.ddd.account.domain.Authority.Aggregate;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.account.domain.Role;
using tdb.ddd.account.domain.Role.Aggregate;
using tdb.ddd.account.domain.User;
using tdb.ddd.account.domain.User.Aggregate;
using tdb.common.Crypto;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.contracts;
using tdb.ddd.account.application.contracts;
using tdb.ddd.account.infrastructure.Config;

namespace tdb.ddd.account.application
{
    /// <summary>
    /// 工具应用
    /// </summary>
    public class ToolsApp : IToolsApp
    {
        #region 初始化数据

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns>日志</returns>
        public async Task<string> InitDataAsync()
        {
            var res = new StringBuilder();

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            // 初始化权限数据
            res.AppendLine(await InitDataAuthorityAsync());

            //初始化角色数据
            res.AppendLine(await InitDataRoleAsync());

            //初始化用户数据
            res.AppendLine(await InitDataUserAsync());

            //提交事务
            TdbRepositoryTran.CommitTran();

            return res.ToString();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public async Task InitDataAsync2()
        {
            TdbLogger.Ins.Info("初始化数据【开始】");

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            // 初始化权限数据
            await InitDataAuthorityAsync();

            //初始化角色数据
            await InitDataRoleAsync();

            //初始化用户数据
            await InitDataUserAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            TdbLogger.Ins.Info("初始化数据【完成】");
        }

        /// <summary>
        /// 初始化权限数据
        /// </summary>
        /// <returns>日志</returns>
        private async Task<string> InitDataAuthorityAsync()
        {
            var res = new StringBuilder();

            TdbLogger.Ins.Info("初始化权限数据【开始】");
            res.AppendLine("初始化权限数据【开始】");

            //权限领域服务
            var authorityService = new AuthorityService();

            #region 账户微服务

            //用户增删改权限
            var userManageAuthority = new AuthorityAgg()
            {
                ID = TdbCst.AuthorityID.AccountUserManage,
                Name = "用户增删改权限",
                Remark = "账户微服务"
            };
            await authorityService.SaveAsync(userManageAuthority);
            TdbLogger.Ins.Info("初始化权限数据（账户微服务->用户增删改权限）");
            res.AppendLine("初始化权限数据（账户微服务->用户增删改权限）");

            #endregion

            TdbLogger.Ins.Info("初始化权限数据【完成】");
            res.AppendLine("初始化权限数据【完成】");

            return res.ToString();
        }

        /// <summary>
        /// 初始化角色数据
        /// </summary>
        /// <returns>日志</returns>
        private async Task<string> InitDataRoleAsync()
        {
            var res = new StringBuilder();

            TdbLogger.Ins.Info("初始化角色数据【开始】");
            res.AppendLine("初始化角色数据【开始】");

            //角色领域服务
            var roleService = new RoleService();

            #region 账户微服务

            //超级管理员
            var superAdminRole = new RoleAgg()
            {
                ID = TdbCst.RoleID.SuperAdmin,
                Name = "超级管理员",
                Remark = "账户微服务"
            };
            superAdminRole.SetLstAuthorityID(new List<long>() { TdbCst.AuthorityID.AccountUserManage });
            await roleService.SaveAsync(superAdminRole);
            TdbLogger.Ins.Info("初始化角色数据（账户微服务->超级管理员）");
            res.AppendLine("初始化角色数据（账户微服务->超级管理员）");

            //账户微服务管理员
            var accountAdminRole = new RoleAgg()
            {
                ID = TdbCst.RoleID.AccountAdmin,
                Name = "账户微服务管理员",
                Remark = "账户微服务"
            };
            accountAdminRole.SetLstAuthorityID(new List<long>() { TdbCst.AuthorityID.AccountUserManage });
            await roleService.SaveAsync(accountAdminRole);
            TdbLogger.Ins.Info("初始化角色数据（账户微服务->账户微服务管理员）");
            res.AppendLine("初始化角色数据（账户微服务->账户微服务管理员）");

            #endregion

            TdbLogger.Ins.Info("初始化角色数据【完成】");
            res.AppendLine("初始化角色数据【完成】");

            return res.ToString();
        }

        /// <summary>
        /// 初始化用户数据
        /// </summary>
        /// <returns>日志</returns>
        private async Task<string> InitDataUserAsync()
        {
            var res = new StringBuilder();

            TdbLogger.Ins.Info("初始化用户数据【开始】");
            res.AppendLine("初始化用户数据【开始】");

            //用户领域服务
            var userService = new UserService();

            #region 账户微服务

            //超级管理员
            var superAdminUser = new UserAgg()
            {
                ID = TdbCst.UserID.SuperAdmin,
                LoginName = AccountConfig.Distributed.InitData.SuperAdminLoginName,
                Password = EncryptHelper.Md5(AccountConfig.Distributed.InitData.SuperAdminDefaultPwd),
                GenderCode = EnmGender.Unknown,
                Birthday = DateTime.Now,
                MobilePhoneValue = new MobilePhoneValueObject() { MobilePhone = AccountConfig.Distributed.InitData.SuperAdminMobilePhone, IsMobilePhoneVerified = true },
                EmailValue = new EmailValueObject() { Email = AccountConfig.Distributed.InitData.SuperAdminEmail, IsEmailVerified = true },
                StatusCode = EnmInfoStatus.Enable,
                Remark = "内置初始用户",
                CreateInfo = new CreateInfoValueObject() { CreatorID = 0, CreateTime = DateTime.Now },
                UpdateInfo = new UpdateInfoValueObject() { UpdaterID = 0, UpdateTime = DateTime.Now }
            };
            superAdminUser.SetName("超级管理员");
            superAdminUser.SetNickName("超级管理员");
            superAdminUser.SetLstRoleID(new List<long>() { TdbCst.RoleID.SuperAdmin });
            await userService.SaveAsync(superAdminUser);
            TdbLogger.Ins.Info("初始化用户数据（账户微服务->超级管理员）");
            res.AppendLine("初始化用户数据（账户微服务->超级管理员）");

            #endregion

            TdbLogger.Ins.Info("初始化用户数据【完成】");
            res.AppendLine("初始化用户数据【完成】");

            return res.ToString();
        }

        #endregion
    }
}
