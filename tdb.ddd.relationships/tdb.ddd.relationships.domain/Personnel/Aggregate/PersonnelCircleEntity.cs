using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;
using tdb.ddd.relationships.domain.contracts.Enum;

namespace tdb.ddd.relationships.domain.Personnel.Aggregate
{
    /// <summary>
    /// 人员圈内信息实体
    /// </summary>
    public class PersonnelCircleEntity : TdbEntity<long>
    {
        #region 值

        /// <summary>
        /// 人际圈ID
        /// </summary>
        public long CircleID { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public EnmRole RoleCode { get; set; }

        /// <summary>
        /// 圈内身份
        /// </summary>
        public string Identity { get; set; } = "";

        #endregion
    }
}
