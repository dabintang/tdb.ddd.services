using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 自动解密ID并绑定模型特效
    /// </summary>
    public class TdbHashIDModelBinderAttribute : ModelBinderAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TdbHashIDModelBinderAttribute() : base(typeof(TdbHashIDModelBinder))
        {

        }
    }
}
