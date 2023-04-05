using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;

namespace tdb.ddd.relationships.domain.Personnel.Aggregate
{
    /// <summary>
    /// 人际圈ID懒加载
    /// </summary>
    public class CircleIDLazyLoad : TdbLazyLoadObject<List<long>>
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

        /// <summary>
        /// 人员聚合
        /// </summary>
        private readonly PersonnelAgg personnelAgg;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="personnelAgg">人员聚合</param>
        public CircleIDLazyLoad(PersonnelAgg personnelAgg)
        {
            this.personnelAgg = personnelAgg;
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        protected override List<long> Load()
        {
            if (this.personnelAgg.ID <= 0)
            {
                throw new TdbException("人际圈ID加载异常，人员ID不正确");
            }

            return this.Repos.GetCircleIDsAsync(this.personnelAgg.ID).Result;
        }
    }
}
