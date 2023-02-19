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
using tdb.ddd.infrastructure.Services;
using tdb.ddd.account.domain.BusMediatR;

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
        /// 初始化权限数据
        /// </summary>
        /// <returns>日志</returns>
        private async Task<string> InitDataAuthorityAsync()
        {
            var res = new StringBuilder();

            var msgStart = "初始化权限数据【开始】";
            TdbLogger.Ins.Info(msgStart);
            res.AppendLine(msgStart);

            //权限领域服务
            var authorityService = new AuthorityService();

            #region 账户微服务

            //用户增删改权限
            if ((await authorityService.IsExist(TdbCst.AuthorityID.AccountUserManage)) == true)
            {
                var msgAccountUserManage = "初始数据已存在（账户微服务->用户增删改权限）";
                TdbLogger.Ins.Info(msgAccountUserManage);
                res.AppendLine(msgAccountUserManage);
            }
            else
            {
                var userManageAuthority = new AuthorityAgg()
                {
                    ID = TdbCst.AuthorityID.AccountUserManage,
                    Name = "用户增删改权限",
                    Remark = "账户微服务"
                };
                await authorityService.SaveAsync(userManageAuthority);

                var msgAccountUserManage = "初始化权限数据（账户微服务->用户增删改权限）";
                TdbLogger.Ins.Info(msgAccountUserManage);
                res.AppendLine(msgAccountUserManage);
            }

            #endregion

            var msgEnd = "初始化权限数据【完成】";
            TdbLogger.Ins.Info(msgEnd);
            res.AppendLine(msgEnd);

            return res.ToString();
        }

        /// <summary>
        /// 初始化角色数据
        /// </summary>
        /// <returns>日志</returns>
        private async Task<string> InitDataRoleAsync()
        {
            var res = new StringBuilder();

            var msgStart = "初始化角色数据【开始】";
            TdbLogger.Ins.Info(msgStart);
            res.AppendLine(msgStart);

            //角色领域服务
            var roleService = new RoleService();

            #region 账户微服务

            //超级管理员
            if ((await roleService.IsExist(TdbCst.RoleID.SuperAdmin)) == true)
            {
                var msgSuperAdmin = "初始数据已存在（账户微服务->超级管理员）";
                TdbLogger.Ins.Info(msgSuperAdmin);
                res.AppendLine(msgSuperAdmin);
            }
            else
            {
                var superAdminRole = new RoleAgg()
                {
                    ID = TdbCst.RoleID.SuperAdmin,
                    Name = "超级管理员",
                    Remark = "账户微服务"
                };
                superAdminRole.SetLstAuthorityID(new List<long>() { TdbCst.AuthorityID.AccountUserManage });
                await roleService.SaveAsync(superAdminRole);

                var msgSuperAdmin = "初始化角色数据（账户微服务->超级管理员）";
                TdbLogger.Ins.Info(msgSuperAdmin);
                res.AppendLine(msgSuperAdmin);
            }

            //账户微服务管理员
            if ((await roleService.IsExist(TdbCst.RoleID.AccountAdmin)) == true)
            {
                var msgAccountAdmin = "初始数据已存在（账户微服务->账户微服务管理员）";
                TdbLogger.Ins.Info(msgAccountAdmin);
                res.AppendLine(msgAccountAdmin);
            }
            else
            {
                var accountAdminRole = new RoleAgg()
                {
                    ID = TdbCst.RoleID.AccountAdmin,
                    Name = "账户微服务管理员",
                    Remark = "账户微服务"
                };
                accountAdminRole.SetLstAuthorityID(new List<long>() { TdbCst.AuthorityID.AccountUserManage });
                await roleService.SaveAsync(accountAdminRole);

                var msgAccountAdmin = "初始化角色数据（账户微服务->账户微服务管理员）";
                TdbLogger.Ins.Info(msgAccountAdmin);
                res.AppendLine(msgAccountAdmin);
            }

            #endregion

            var msgEnd = "初始化角色数据【完成】";
            TdbLogger.Ins.Info(msgEnd);
            res.AppendLine(msgEnd);

            return res.ToString();
        }

        /// <summary>
        /// 初始化用户数据
        /// </summary>
        /// <returns>日志</returns>
        private async Task<string> InitDataUserAsync()
        {
            var res = new StringBuilder();

            var msgStart = "初始化用户数据【开始】";
            TdbLogger.Ins.Info(msgStart);
            res.AppendLine(msgStart);

            //用户领域服务
            var userService = new UserService();

            #region 账户微服务

            //超级管理员
            if ((await userService.IsExistUserID(TdbCst.UserID.SuperAdmin)) == true)
            {
                var msgSuperAdmin = "初始数据已存在（账户微服务->超级管理员）";
                TdbLogger.Ins.Info(msgSuperAdmin);
                res.AppendLine(msgSuperAdmin);
            }
            else
            {
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

                var msgSuperAdmin = "初始化用户数据（账户微服务->超级管理员）";
                TdbLogger.Ins.Info(msgSuperAdmin);
                res.AppendLine(msgSuperAdmin);
            }

            #endregion

            var msgEnd = "初始化用户数据【完成】";
            TdbLogger.Ins.Info(msgEnd);
            res.AppendLine(msgEnd);

            return res.ToString();
        }

        #endregion
    }
}
