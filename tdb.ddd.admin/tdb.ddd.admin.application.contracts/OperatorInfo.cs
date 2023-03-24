using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.admin.application.contracts
{
    /// <summary>
    /// 操作人信息
    /// </summary>
    public class OperatorInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>           
        public long ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>           
        public string Name { get; set; } = "";

        /// <summary>
        /// 用户拥有的角色ID
        /// </summary>
        public List<long> LstRoleID { get; set; } = new List<long>();

        /// <summary>
        /// 用户拥有的权限ID
        /// </summary>
        public List<long> LstAuthorityID { get; set; } = new List<long>();
    }
}
