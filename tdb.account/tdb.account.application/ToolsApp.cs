using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.domain.Authority;
using tdb.account.domain.Authority.Aggregate;
using tdb.account.domain.contracts.Const;
using tdb.account.domain.contracts.Enum;
using tdb.account.domain.Role;
using tdb.account.domain.Role.Aggregate;
using tdb.account.domain.User;
using tdb.account.domain.User.Aggregate;
using tdb.common.Crypto;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.account.application
{
    /// <summary>
    /// 工具
    /// </summary>
    public class ToolsApp
    {
        #region 初始化数据

        /// <summary>
        /// 初始化数据
        /// </summary>
        public static void InitData()
        {
            TdbLogger.Ins.Info("初始化数据【开始】");

            /*
             * TODO：打印【SqlSugarScope.ContextID】,看异步操作下数据库上下文是否一致。（debug、release）
             */

            // 初始化权限数据
            InitDataAuthority();

            //初始化角色数据
            InitDataRole();

            //初始化用户数据
            InitDataUser();

            TdbLogger.Ins.Info("初始化数据【完成】");
        }

        /// <summary>
        /// 初始化权限数据
        /// </summary>
        private static void InitDataAuthority()
        {
            TdbLogger.Ins.Info("初始化权限数据【开始】");

            //权限领域服务
            var authorityService = new AuthorityService();

            #region 账户微服务

            //用户增删改权限
            var userManageAuthority = new AuthorityAgg()
            {
                ID = Cst.AuthorityID.AccountUserManage,
                Name = "用户增删改权限",
                Remark = "账户微服务"
            };
            authorityService.SaveAsync(userManageAuthority).Wait();
            TdbLogger.Ins.Info("初始化权限数据（账户微服务->用户增删改权限）");

            #endregion

            TdbLogger.Ins.Info("初始化权限数据【完成】");
        }

        /// <summary>
        /// 初始化角色数据
        /// </summary>
        private static void InitDataRole()
        {
            TdbLogger.Ins.Info("初始化角色数据【开始】");

            //角色领域服务
            var roleService = new RoleService();

            #region 账户微服务

            //超级管理员
            var superAdminRole = new RoleAgg()
            {
                ID = Cst.RoleID.SuperAdmin,
                Name = "超级管理员",
                Remark = "账户微服务"
            };
            superAdminRole.LstAuthorityID.Value = new List<int>() { Cst.AuthorityID.AccountUserManage };
            roleService.SaveAsync(superAdminRole).Wait();
            TdbLogger.Ins.Info("初始化角色数据（账户微服务->超级管理员）");

            //账户微服务管理员
            var accountAdminRole = new RoleAgg()
            {
                ID = Cst.RoleID.AccountAdmin,
                Name = "账户微服务管理员",
                Remark = "账户微服务"
            };
            accountAdminRole.LstAuthorityID.Value = new List<int>() { Cst.AuthorityID.AccountUserManage };
            roleService.SaveAsync(accountAdminRole).Wait();
            TdbLogger.Ins.Info("初始化角色数据（账户微服务->账户微服务管理员）");

            #endregion

            TdbLogger.Ins.Info("初始化角色数据【完成】");
        }

        /// <summary>
        /// 初始化用户数据
        /// </summary>
        private static void InitDataUser()
        {
            TdbLogger.Ins.Info("初始化用户数据【开始】");

            //用户领域服务
            var userService = new UserService();

            #region 账户微服务

            //超级管理员
            var superAdminUser = new UserAgg()
            {
                ID = Cst.UserID.SuperAdmin,
                Name = "超级管理员",
                NickName = "超级管理员",
                LoginName = "superadmin",
                Password = EncryptHelper.Md5("8888888"),
                GenderCode = EnmGender.Unknown,
                Birthday = DateTime.Now,
                MobilePhoneValue = new MobilePhoneValueObject() { MobilePhone = "13788576707", IsMobilePhoneVerified = true },
                EmailValue = new EmailValueObject() { Email = "187235004@qq.com", IsEmailVerified = true },
                StatusCode = EnmInfoStatus.Enable,
                Remark = "内置初始用户",
                CreateInfo = new CreateInfoValueObject() { CreatorID = 0, CreateTime = DateTime.Now },
                UpdateInfo = new UpdateInfoValueObject() { UpdaterID = 0, UpdateTime = DateTime.Now }
            };
            superAdminUser.LstRoleID.Value = new List<int>() { Cst.RoleID.SuperAdmin };
            userService.SaveAsync(superAdminUser).Wait();
            TdbLogger.Ins.Info("初始化用户数据（账户微服务->超级管理员）");

            #endregion

            TdbLogger.Ins.Info("初始化用户数据【完成】");
        }

        #endregion
    }
}
