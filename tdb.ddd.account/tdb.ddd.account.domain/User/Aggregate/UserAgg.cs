using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.BusMediatR;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.account.domain.contracts.User;
using tdb.ddd.account.infrastructure.Config;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using System.Text.Json.Serialization;

namespace tdb.ddd.account.domain.User.Aggregate
{
    /// <summary>
    /// 用户聚合
    /// </summary>
    public class UserAgg : TdbAggregateRoot<long>
    {
        #region 值

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; internal set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码(MD5)
        /// </summary>
        public string Password { get; set; }

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
        public long? HeadImgID { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public MobilePhoneValueObject MobilePhoneValue { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public EmailValueObject EmailValue { get; set; }

        /// <summary>
        /// 状态（1：激活；2：禁用）
        /// </summary>
        public EnmInfoStatus StatusCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建信息
        /// </summary>
        public CreateInfoValueObject CreateInfo { get; set; }

        /// <summary>
        /// 更新信息
        /// </summary>
        public UpdateInfoValueObject UpdateInfo { get; set; }

        /// <summary>
        /// 用户拥有的角色ID
        /// </summary>
        [JsonIgnore]
        public UserRoleIDLazyLoad LstRoleID { get; private set; }
        
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserAgg()
        {
            this.LstRoleID = new UserRoleIDLazyLoad(this);
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
        /// 登录
        /// </summary>
        /// <param name="clientIP">客户端IP</param>
        /// <returns></returns>
        public async Task<TdbRes<UserLoginResult>> LoginAsync(string clientIP)
        {
            //用户有效性
            if (this.StatusCode == EnmInfoStatus.Disable)
            {
                return new TdbRes<UserLoginResult>(AccountConfig.Msg.Disableduser, null);
            }

            //用户权限ID
            var lstAuthorityID = new List<long>();
            foreach (var roleID in this.LstRoleID.Value)
            {
                //获取角色权限ID
                var lstRoleAuthorityID = await TdbMediatR.Send(new GetRoleAuthorityIDRequest() { RoleID = roleID });
                lstAuthorityID.AddRange(lstRoleAuthorityID);
            }

            //响应结果
            var res = new UserLoginResult
            {
                AccessToken = CreateAccessToken(lstAuthorityID, clientIP),
                AccessTokenValidSeconds = AccountConfig.Distributed.Token.AccessTokenValidSeconds,
                RefreshToken = Guid.NewGuid().ToString("N"),
                RefreshTokenValidSeconds = AccountConfig.Distributed.Token.RefreshTokenValidSeconds
            };

            //缓存刷新令牌
            TdbCache.Ins.Set(res.RefreshToken, this.ID, TimeSpan.FromSeconds(res.RefreshTokenValidSeconds));

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 赋予角色（全量赋值）
        /// </summary>
        /// <param name="lstRoleID">角色ID</param>
        public TdbRes<bool> SetLstRoleID(List<long> lstRoleID)
        {
            lstRoleID ??= new List<long>();
            return this.LstRoleID.SetValue(lstRoleID);
        }

        /// <summary>
        /// 设置姓名
        /// </summary>
        /// <param name="name">姓名</param>
        public void SetName(string name)
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
        public void SetNickName(string nickName)
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
        public void SetMobilePhone(string mobilePhone)
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
        public void SetEmail(string email)
        {
            if (this.EmailValue?.Email == email)
            {
                return;
            }

            this.EmailValue = new EmailValueObject() { Email = email ?? "", IsEmailVerified = false };
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 生成访问令牌
        /// </summary>
        /// <param name="lstAuthorityID">用户权限ID</param>
        /// <param name="clientIP">客户端IP</param>
        /// <returns>token</returns>
        private string CreateAccessToken(List<long> lstAuthorityID, string clientIP)
        {
            //用户基本信息
            var lstClaim = new List<Claim>
            {
                new Claim(TdbClaimTypes.UID, this.ID.ToStr()),
                new Claim(TdbClaimTypes.UName, this.Name),
                //客户端IP
                new Claim(TdbClaimTypes.ClientIP, clientIP),
                //角色ID
                new Claim(TdbClaimTypes.RoleID, this.LstRoleID.Value.SerializeJson()),
                //权限ID
                new Claim(TdbClaimTypes.AuthorityID, lstAuthorityID.SerializeJson())
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(lstClaim),
                Issuer = AccountConfig.Common.Token.Issuer,
                //Audience = AccConfig.Consul.Token.Audience,
                Expires = DateTime.UtcNow.AddSeconds(AccountConfig.Distributed.Token.AccessTokenValidSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AccountConfig.Common.Token.SecretKey)), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        #endregion
    }
}
