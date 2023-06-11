using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using System.Text.Json.Serialization;
using tdb.ddd.account.domain.BusMediatR;
using tdb.ddd.infrastructure.Services;

namespace tdb.ddd.account.domain.User.Aggregate
{
    /// <summary>
    /// 用户聚合
    /// </summary>
    public class UserAgg : TdbAggregateRoot<long>
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

        #region 值

        /// <summary>
        /// 姓名
        /// </summary>
        [JsonInclude]
        public string Name { get; internal set; } = "";

        /// <summary>
        /// 昵称
        /// </summary>
        [JsonInclude]
        public string NickName { get; internal set; } = "";

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; } = "";

        /// <summary>
        /// 密码(MD5)
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// 性别（1：男；2：女；3：未知）
        /// </summary>
        public EnmGender GenderCode { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 头像图片ID
        /// </summary>
        [JsonInclude]
        public long? HeadImgID { get; internal set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public MobilePhoneValueObject MobilePhoneValue { get; set; } = new MobilePhoneValueObject();

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public EmailValueObject EmailValue { get; set; } = new EmailValueObject();

        /// <summary>
        /// 状态（1：激活；2：禁用）
        /// </summary>
        public EnmInfoStatus StatusCode { get; set; }

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

        /// <summary>
        /// 头像图片修改信息
        /// </summary>
        private HeadImgChangedInfo? headImgChangedInfo;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserAgg()
        {
        }

        #region 行为

        /// <summary>
        /// 是否已被禁用
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                return this.StatusCode == EnmInfoStatus.Disable;
            }
        }

        /// <summary>
        /// 设置姓名
        /// </summary>
        /// <param name="name">姓名</param>
        public void SetName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                this.Name = TdbHashID.EncodeLong(this.ID);
            }
            else
            {
                this.Name = name;
            }
        }

        /// <summary>
        /// 设置昵称
        /// </summary>
        /// <param name="nickName">昵称</param>
        public void SetNickName(string? nickName)
        {
            if (string.IsNullOrWhiteSpace(nickName))
            {
                this.NickName = TdbHashID.EncodeLong(this.ID);
            }
            else
            {
                this.NickName = nickName;
            }
        }

        /// <summary>
        /// 设置手机号码
        /// </summary>
        /// <param name="mobilePhone">手机号码</param>
        public void SetMobilePhone(string? mobilePhone)
        {
            if (this.MobilePhoneValue?.MobilePhone == mobilePhone)
            {
                return;
            }

            this.MobilePhoneValue = new MobilePhoneValueObject() { MobilePhone = mobilePhone ?? "", IsMobilePhoneVerified = false };
        }

        /// <summary>
        /// 设置电子邮箱
        /// </summary>
        /// <param name="email">电子邮箱</param>
        public void SetEmail(string? email)
        {
            if (this.EmailValue?.Email == email)
            {
                return;
            }

            this.EmailValue = new EmailValueObject() { Email = email ?? "", IsEmailVerified = false };
        }

        /// <summary>
        /// 设置头像图片ID
        /// </summary>
        /// <param name="headImgID">头像图片ID</param>
        public void SetHeadImgID(long? headImgID)
        {
            this.headImgChangedInfo ??= new HeadImgChangedInfo() { OldHeadImgID = this.HeadImgID };
            this.HeadImgID = headImgID;
        }

        /// <summary>
        /// 获取用户拥有的角色ID
        /// </summary>
        /// <returns></returns>
        public async Task<List<long>> GetRoleIDsAsync()
        {
            return await this.UserRepos.GetRoleIDsAsync(this.ID);
        }

        /// <summary>
        /// 赋予角色并保存（全量赋值）
        /// </summary>
        /// <param name="lstRoleID">角色ID</param>
        /// <returns></returns>
        public async Task SetRoleAndSaveAsync(List<long> lstRoleID)
        {
            await this.UserRepos.SaveUserRoleAsync(this.ID, lstRoleID);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await this.UserRepos.SaveAsync(this);

            if (this.headImgChangedInfo is not null)
            {
                //通知头像有改动
                var msg = new UserHeadImgChangedNotification()
                {
                    OldHeadImgID = this.headImgChangedInfo.OldHeadImgID,
                    NewHeadImgID = this.HeadImgID,
                    OperatorID = this.UpdateInfo.UpdaterID,
                    OperationTime = this.UpdateInfo.UpdateTime
                };
                TdbMediatR.Publish(msg);
            }
        }

        #endregion

        #region 内部类

        /// <summary>
        /// 头像图片ID修改信息
        /// </summary>
        class HeadImgChangedInfo
        {
            /// <summary>
            /// 原头像图片ID
            /// </summary>
            public long? OldHeadImgID { get; set;}
        }

        #endregion
    }
}
