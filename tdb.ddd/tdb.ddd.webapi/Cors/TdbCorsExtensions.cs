namespace tdb.ddd.webapi
{
    /// <summary>
    /// 跨域设置
    /// </summary>
    public static class TdbCorsExtensions
    {
        /// <summary>
        /// 允许所有跨域请求策略名
        /// </summary>
        public const string AllAllowCorsPolicyName = "TdbAllAllowCorsPolicy";

        /// <summary>
        /// 允许所有来源跨域
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <returns></returns>
        public static IServiceCollection AddTdbCorsAllAllow(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy(AllAllowCorsPolicyName, builder =>
            {
                builder.WithOrigins("urls")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true) // =AllowAnyOrigin()
                    .AllowCredentials();
            })
            );

            return services;
        }

        /// <summary>
        /// 允许所有来源跨域
        /// </summary>
        /// <param name="app">管道配置类</param>
        /// <returns></returns>
        public static IApplicationBuilder UseTdbCorsAllAllow(this IApplicationBuilder app)
        {
            // 跨域设置
            return app.UseCors(AllAllowCorsPolicyName);
        }
    }
}
