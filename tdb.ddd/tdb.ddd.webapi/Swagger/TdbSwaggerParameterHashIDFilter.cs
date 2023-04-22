using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using tdb.ddd.application;
using tdb.ddd.application.contracts;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 标记[TdbHashIDModelBinderAttribute]特性的属性，修改为string型
    /// </summary>
    public class TdbSwaggerParameterHashIDFilter : IParameterFilter
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            //是否标识有[TdbHashIDModelBinderAttribute]特性
            if (context.PropertyInfo?.HasAttribute<TdbHashIDModelBinderAttribute>() == false)
            {
                return;
            }

            //转为HashID schema
            parameter.Schema = TdbSwaggerHelper.ToHashIDSchema(parameter.Schema);
        }
    }
}
