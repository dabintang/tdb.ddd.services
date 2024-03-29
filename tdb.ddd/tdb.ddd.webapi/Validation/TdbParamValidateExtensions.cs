﻿using Microsoft.AspNetCore.Mvc;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 参数验证
    /// </summary>
    public static class TdbParamValidateExtensions
    {
        /// <summary>
        /// 添加参数验证处理
        /// </summary>
        /// <param name="services"></param>
        public static void AddTdbParamValidate(this IServiceCollection services)
        {
            //抑制模型验证过滤器
            services.Configure<ApiBehaviorOptions>(opts => opts.SuppressModelStateInvalidFilter = true);

            //添加参数验证过滤器
            services.AddMvcCore(options => {
                options.Filters.Add<TdbParamValidateFilter>();
            });
        }
    }
}
