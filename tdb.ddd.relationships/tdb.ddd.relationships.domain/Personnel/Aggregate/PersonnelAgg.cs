using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.domain.contracts.Enum;

namespace tdb.ddd.relationships.domain.Personnel.Aggregate
{
    /// <summary>
    /// 人员聚合
    /// </summary>
    public class PersonnelAgg : TdbAggregateRoot<long>
    {
        #region 仓储

        private IPersonnelRepos? _repos;
        /// <summary>
        /// 人员仓储
        /// </summary>
        private IPersonnelRepos Repos
        {
            get
            {
                this._repos ??= TdbIOC.GetService<IPersonnelRepos>();
                if (this._repos is null)
                {
                    throw new TdbException("人员仓储接口未实现");
                }
                return this._repos;
            }
        }

        #endregion

        #region 值

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 性别
        /// </summary>
        public EnmGender GenderCode { get; set; }

        /// <summary>
        /// 头像图片ID
        /// </summary>
        public long? HeadImgID { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; } = "";

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 用户账户ID
        /// </summary>
        public long? UserID { get; set; }

        /// <summary>
        /// 创建信息
        /// </summary>
        public CreateInfoValueObject CreateInfo { get; set; } = new CreateInfoValueObject();

        /// <summary>
        /// 更新信息
        /// </summary>
        public UpdateInfoValueObject UpdateInfo { get; set; } = new UpdateInfoValueObject();

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public PersonnelAgg()
        {
        }

        #region 行为

        /// <summary>
        /// 获取人员所在人际圈ID
        /// </summary>
        /// <returns></returns>
        public async Task<List<long>> GetCircleIDsAsync()
        {
             return await this.Repos.GetCircleIDsAsync(this.ID);
        }

        /// <summary>
        /// 获取人员所有照片ID
        /// </summary>
        /// <returns></returns>
        public async Task<List<long>> GetPhotoIDsAsync()
        {
            return await this.Repos.GetPhotoIDsAsync(this.ID);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await this.Repos.SaveChangedAsync(this);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            await this.Repos.DeleteAsync(this);
        }

        #endregion

        #region 私有方法

        #endregion
    }
}
