using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.domain.contracts.Personnel;
using tdb.ddd.relationships.domain.Personnel.Aggregate;

namespace tdb.ddd.relationships.domain.Personnel
{
    /// <summary>
    /// 人员领域服务
    /// </summary>
    public class PersonnelService
    {
        #region 仓储

        private IPersonnelRepos? _personnelRepos;
        /// <summary>
        /// 人员仓储
        /// </summary>
        private IPersonnelRepos PersonnelRepos
        {
            get
            {
                this._personnelRepos ??= TdbIOC.GetService<IPersonnelRepos>();
                if (this._personnelRepos is null)
                {
                    throw new TdbException("人员仓储接口未实现");
                }
                return this._personnelRepos;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 根据ID获取人员聚合
        /// </summary>
        /// <param name="personnelID">人员ID</param>
        /// <returns></returns>
        public async Task<PersonnelAgg?> GetByIDAsync(long personnelID)
        {
            //获取人员聚合
            var agg = await this.PersonnelRepos.GetByIDAsync(personnelID);
            return agg;
        }

        /// <summary>
        /// 根据用户ID获取人员聚合
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public async Task<PersonnelAgg?> GetByUserIDAsync(long userID)
        {
            //获取照片聚合
            var agg = await this.PersonnelRepos.GetByUserIDAsync(userID);
            return agg;
        }

        /// <summary>
        /// 查询人员聚合
        /// </summary>
        /// <param name="param">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<PersonnelAgg>> QueryAsync(QueryPersonnelParam param)
        {
            var list = await this.PersonnelRepos.QueryAsync(param);
            return list;
        }

        #endregion
    }
}
