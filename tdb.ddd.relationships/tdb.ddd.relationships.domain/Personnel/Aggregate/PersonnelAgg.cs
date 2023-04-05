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
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建信息
        /// </summary>
        public CreateInfoValueObject CreateInfo { get; set; } = new CreateInfoValueObject();

        /// <summary>
        /// 更新信息
        /// </summary>
        public UpdateInfoValueObject UpdateInfo { get; set; } = new UpdateInfoValueObject();

        /// <summary>
        /// 人员照片ID
        /// </summary>
        public PhotoIDLazyLoad LstPhotoID { get; }

        /// <summary>
        /// 人际圈ID
        /// </summary>
        public CircleIDLazyLoad LstCircleID { get; }

        /// <summary>
        /// 记录下被操作过的人员圈内信息
        /// </summary>
        internal List<PersonnelCircleEntity> LstOperatedPersonnelCircleInfo = new();

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public PersonnelAgg()
        {
            this.LstPhotoID = new PhotoIDLazyLoad(this);
            this.LstCircleID = new CircleIDLazyLoad(this);
        }

        #region 行为

        /// <summary>
        /// 获取人员在指定人际圈内信息
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <returns></returns>
        public async Task<PersonnelCircleEntity> GetPersonnelCircleInfoAsync(long circleID)
        {
            var personnelCircleInfo = this.LstOperatedPersonnelCircleInfo.Find(m => m.CircleID == circleID);
            if (personnelCircleInfo is not null)
            {
                return personnelCircleInfo;
            }

            return await this.Repos.GetPersonnelCircleInfoAsync(this.ID, circleID);
        }

        /// <summary>
        /// 设置圈内角色
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <param name="roleCode">角色编码</param>
        public async Task SetRole(long circleID, EnmRole roleCode)
        {
            //获取人员在指定人际圈内信息
            var personnelCircleInfo = await this.GetPersonnelCircleInfoAsync(circleID);
            if (personnelCircleInfo is null)
            {
                throw new TdbException($"人员不在交际圈[{circleID}]内，无法设置角色");
            }

            //设置圈内角色
            personnelCircleInfo.RoleCode = roleCode;
            //记录下设置的人员圈内信息
            this.SetPersonnelCircle(personnelCircleInfo);
        }

        /// <summary>
        /// 设置圈内身份
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <param name="identity">身份</param>
        public async Task SetIdentity(long circleID, string? identity)
        {
            //获取人员在指定人际圈内信息
            var personnelCircleInfo = await this.GetPersonnelCircleInfoAsync(circleID);
            if (personnelCircleInfo is null)
            {
                throw new TdbException($"人员不在交际圈[{circleID}]内，无法设置身份");
            }

            //设置圈内身份
            personnelCircleInfo.Identity = identity ?? "";
            //记录下设置的人员圈内信息
            this.SetPersonnelCircle(personnelCircleInfo);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangedAsync()
        {
            await this.Repos.SaveChangedAsync(this);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            await this.Repos.DeleteAsync(this.ID);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 记录下设置的人员圈内信息
        /// </summary>
        /// <param name="entity">人员圈内信息</param>
        private void SetPersonnelCircle(PersonnelCircleEntity entity)
        {
            //移除该信息原来的操作
            this.LstOperatedPersonnelCircleInfo.RemoveAll(m => m.ID == entity.ID);
            //添加本次操作
            this.LstOperatedPersonnelCircleInfo.Add(entity);
        }

        #endregion
    }
}
