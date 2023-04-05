using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

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
        /// 创建信息
        /// </summary>
        public CreateInfoValueObject CreateInfo { get; set; } = new CreateInfoValueObject();

        /// <summary>
        /// 更新信息
        /// </summary>
        public UpdateInfoValueObject UpdateInfo { get; set; } = new UpdateInfoValueObject();

        /// <summary>
        /// 人员ID
        /// </summary>
        public PersonnelIDLazyLoad LstPersonnelID { get; }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public CircleAgg()
        {
            this.LstPersonnelID = new PersonnelIDLazyLoad(this);
        }

        #region 行为

        /// <summary>
        /// 把人员加入人际圈
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        public void AddPersonnel(long personnelID)
        {
            var lstPersonnelID = this.LstPersonnelID.Value!;
            lstPersonnelID.Add(personnelID);
        }

        /// <summary>
        /// 把人员移出人际圈
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        public void RemovePersonnel(long personnelID)
        {
            var lstPersonnelID = this.LstPersonnelID.Value!;
            lstPersonnelID.Remove(personnelID);
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
    }
}
