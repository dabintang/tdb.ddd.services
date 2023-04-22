using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.domain.Circle.Aggregate;
using tdb.ddd.relationships.domain.Personnel.Aggregate;

namespace tdb.ddd.relationships.domain.Circle
{
    /// <summary>
    /// 人际圈领域服务
    /// </summary>
    public class CircleService
    {
        #region 仓储

        private ICircleRepos? _personnelRepos;
        /// <summary>
        /// 人际圈仓储
        /// </summary>
        private ICircleRepos CircleRepos
        {
            get
            {
                this._personnelRepos ??= TdbIOC.GetService<ICircleRepos>();
                if (this._personnelRepos is null)
                {
                    throw new TdbException("人际圈仓储接口未实现");
                }
                return this._personnelRepos;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 根据ID获取人际圈聚合
        /// </summary>
        /// <param name="circleID">人际圈ID</param>
        /// <returns></returns>
        public async Task<CircleAgg?> GetByIDAsync(long circleID)
        {
            //获取人际圈聚合
            var agg = await this.CircleRepos.GetByIDAsync(circleID);
            return agg;
        }

        #endregion
    }
}
