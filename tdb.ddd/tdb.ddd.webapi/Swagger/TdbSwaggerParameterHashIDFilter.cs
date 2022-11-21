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


        ///// <summary>
        ///// Apply
        ///// </summary>
        ///// <param name="schema"></param>
        ///// <param name="context"></param>
        //public void Apply2(OpenApiSchema schema, SchemaFilterContext context)
        //{
        //    //属性为空，不处理
        //    if (schema?.Properties?.IsNullOrEmpty() ?? true)
        //    {
        //        return;
        //    }

        //    var properties = context.Type.GetProperties();
        //    foreach (var property in properties)
        //    {
        //        //是否标识有[TdbHashIDJsonConverterAttribute]特性
        //        if (schema.Properties.ContainsKey(property.Name) && property.HasAttribute<TdbHashIDJsonConverterAttribute>())
        //        {
        //            //原ID schema
        //            var originalSchema = schema.Properties[property.Name];
        //            //转为HashID schema
        //            var hashIDSchema = TdbSwaggerHelper.ToHashIDSchema(originalSchema);
        //            //替换原来的 schema
        //            schema.Properties[property.Name] = hashIDSchema;
        //        }
        //    }
        //}
    }
}
