using Microsoft.OpenApi.Models;
using tdb.common;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// swagger帮助类
    /// </summary>
    public static class TdbSwaggerHelper
    {
        /// <summary>
        /// 转成HashID schema （long型改为string型）
        /// </summary>
        /// <param name="original">原始的ID schema</param>
        /// <returns></returns>
        public static OpenApiSchema? ToHashIDSchema(OpenApiSchema original)
        {
            //深复制
            var strHashID = original.DeepClone();
            if (strHashID != null)
            {
                //修改类型
                strHashID.Type = "string";
                strHashID.Format = "string";
            }

            return strHashID;
        }

        /// <summary>
        /// 转成HashIDList schema （long型改为string列表类型）
        /// </summary>
        /// <param name="original">原始的ID schema</param>
        /// <returns></returns>
        public static OpenApiSchema? ToHashIDListSchema(OpenApiSchema original)
        {
            //深复制
            var strHashIDList = original.DeepClone();
            if (strHashIDList != null)
            {
                //修改类型
                strHashIDList.Type = "array";
                strHashIDList.Format = "array";
                strHashIDList.Items.Type = "string";
                strHashIDList.Items.Format = "string";
            }

            return strHashIDList;
        }
    }
}
