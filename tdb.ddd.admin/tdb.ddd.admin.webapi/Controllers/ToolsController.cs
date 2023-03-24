using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.contracts;
using tdb.ddd.webapi;

namespace tdb.ddd.admin.webapi.Controllers
{
    /// <summary>
    /// 一些帮助工具
    /// </summary>
    [TdbApiVersion(1)]
    public class ToolsController : BaseController
    {
        #region 接口

        /// <summary>
        /// id转hash id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpGet]
        public string IDToHashID(long id)
        {
            return TdbHashID.EncodeLong(id);
        }

        /// <summary>
        /// hash id转id
        /// </summary>
        /// <param name="hashID">hash id</param>
        /// <returns></returns>
        [HttpGet]
        public long HashIDToID(string hashID)
        {
            return TdbHashID.DecodeSingleLong(hashID);
        }

        #endregion
    }
}
