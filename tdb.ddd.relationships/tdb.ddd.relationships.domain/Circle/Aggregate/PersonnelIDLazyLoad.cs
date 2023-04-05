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
    /// 人员ID懒加载
    /// </summary>
    public class PersonnelIDLazyLoad : TdbLazyLoadObject<List<long>>
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

        /// <summary>
        /// 人际圈聚合
        /// </summary>
        private readonly CircleAgg circleAgg;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="circleAgg">人际圈聚合</param>
        public PersonnelIDLazyLoad(CircleAgg circleAgg)
        {
            this.circleAgg = circleAgg;
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        protected override List<long> Load()
        {
            if (this.circleAgg.ID <= 0)
            {
                throw new TdbException("人员ID加载异常，人际圈ID不正确");
            }

            return this.Repos.GetPersonnelIDsAsync(this.circleAgg.ID).Result;
        }
    }
}
