using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using tdb.common.Crypto;
using tdb.ddd.account.domain.contracts.Enum;
using tdb.ddd.account.infrastructure.Config;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.domain.Certificate.Aggregate
{
    /// <summary>
    /// 凭证聚合
    /// </summary>
    public class CertificateAgg : TdbAggregateRoot<long>
    {
        #region 仓储

        private ICertificateRepos? _certificateRepos;
        /// <summary>
        /// 凭证仓储
        /// </summary>
        private ICertificateRepos CertificateRepos
        {
            get
            {
                this._certificateRepos ??= TdbIOC.GetService<ICertificateRepos>();
                if (this._certificateRepos is null)
                {
                    throw new TdbException("凭证仓储接口未实现");
                }

                return this._certificateRepos;
            }
        }

        #endregion

        #region 值

        /// <summary>
        /// 凭证类型
        /// </summary>
        [JsonInclude]
        public EnmCertificateType CertificateTypeCode { get; internal set; }

        // <summary>
        /// 凭证内容
        /// </summary>
        [JsonInclude]
        public string Credentials { get; internal set; } = "";

        /// <summary>
        /// 创建信息
        /// </summary>
        [JsonInclude]
        public CreateInfoValueObject CreateInfo { get; internal set; } = new CreateInfoValueObject();

        #endregion

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public CertificateAgg()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="certificateTypeCode">凭证类型</param>
        /// <param name="credentials">凭证内容</param>
        /// <param name="creatorID">创建人ID</param>
        /// <param name="createTime">创建时间</param>
        public CertificateAgg(EnmCertificateType certificateTypeCode, string credentials, long creatorID, DateTime createTime)
        {
            this.CertificateTypeCode = certificateTypeCode;
            this.CreateInfo.CreatorID = creatorID;
            this.CreateInfo.CreateTime = createTime;

            if (credentials.Length > 32)
            {
                credentials = EncryptHelper.Md5(credentials);
            }
            this.Credentials = credentials;
        }

        #region 行为

        /// <summary>
        /// 获取凭证绑定的用户ID
        /// </summary>
        /// <returns></returns>
        public async Task<long?> GetUserIDAsync()
        {
            //获取凭证绑定的用户ID集合
            var lstUserID = await this.CertificateRepos.GetUserIDsAsync(this.ID);
            if (lstUserID.Count > 0)
            {
                return lstUserID[0];
            }
            return null;
        }

        /// <summary>
        /// 绑定用户
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> BindingUserAsync(long userID)
        {
            //获取凭证绑定的用户ID集合
            var lstUserID = await this.CertificateRepos.GetUserIDsAsync(this.ID);
            if (lstUserID.Count > 0)
            {
                if (lstUserID.Contains(userID))
                {
                    return TdbRes.Success(true);
                }

                //凭证已绑定其他用户
                return new TdbRes<bool>(AccountConfig.Msg.CertificateHadBindUser, false);
            }

            //绑定用户
            await this.CertificateRepos.BindingUserAsync(this.ID, userID);
            return TdbRes.Success(true);
        }

        /// <summary>
        /// 解绑所有用户
        /// </summary>
        /// <returns></returns>
        private async Task UnbindingAllUserAsync()
        {
            //解绑所有用户
            await this.CertificateRepos.UnbindingAllUserAsync(this.ID);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            //保存凭证
            await this.CertificateRepos.SaveAsync(this);
        }

        /// <summary>
        /// 删除凭证
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            //解绑所有用户
            await this.UnbindingAllUserAsync();
            //删除凭证
            await this.CertificateRepos.DeleteAsync(this);
        }

        #endregion
    }
}
