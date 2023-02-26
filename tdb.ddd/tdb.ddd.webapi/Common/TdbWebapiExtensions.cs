using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;
using static tdb.ddd.webapi.Common.TdbWebAppBuilderOption;

namespace tdb.ddd.webapi.Common
{
    /// <summary>
    /// 添加服务及使用
    /// </summary>
    public static class TdbWebapiExtensions
    {
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="builder">A builder for web applications and services.</param>
        /// <param name="setupOption">配置选项</param>
        public static void RunWebApp(this WebApplicationBuilder builder, Action<TdbWebAppBuilderOption> setupOption)
        {
            //选项
            var option = new TdbWebAppBuilderOption();
            //配置选项
            setupOption(option);

            #region 日志

            switch (option.LogOption.EnmLog)
            {
                case TdbEnmLog.NLog:
                    if (option.LogOption.NLogOption is null)
                    {
                        throw new TdbException("使用nlog日志时，NLogOption选项不能为空");
                    }

                    //使用nlog
                    UseNlog(builder, option.LogOption.NLogOption);
                    break;
                default:
                    throw new TdbException("不支持的日志类型");
            }

            #endregion

            #region IOC

            switch (option.IOCOption.EnmIOC)
            {
                case TdbEnmIOC.Autofac:
                    {
                        if (option.IOCOption.AutofacOption is null)
                        {
                            throw new TdbException("使用autofac容器时，AutofacOption选项不能为空");
                        }

                        //使用autofac
                        UseAutofac(builder, option.IOCOption.AutofacOption);
                    }
                    break;
                default:
                    throw new TdbException("不支持的IOC类型");
            }

            #endregion

            //初始化配置
            if (option.InitConfigAction is not null)
            {
                InitConfig(option.InitConfigAction);
            }

            //hash id
            if (option.ConfigureHashIDAction is not null)
            {
                InitHashID(option.ConfigureHashIDAction);
            }

            #region 缓存

            switch (option.CacheOption.EnmCache)
            {
                case TdbEnmCache.Memory:
                    {
                        //使用内存缓存服务
                        UseMemoryCache(builder, option.CacheOption.ConfigureMemoryAction);
                    }
                    break;
                case TdbEnmCache.Redis:
                    {
                        if (option.CacheOption.ConfigureRedisAction is null)
                        {
                            throw new TdbException("使用redis缓存时，RedisOption选项不能为空");
                        }

                        //使用redis缓存
                        UseRedisCache(builder, option.CacheOption.ConfigureRedisAction);
                    }
                    break;
                default:
                    throw new TdbException("不支持的缓存类型");
            }

            #endregion

            #region 总线

            if (option.BusOption.IsUseMediatR)
            {
                if (option.BusOption.MediatROption is null)
                {
                    throw new TdbException("使用MediatR时，MediatROption选项不能为空");
                }

                UseMediatR(builder, option.BusOption.MediatROption);
            }

            #endregion

            //SqlSugar
            if (option.ConfigureSqlSugarAction is not null)
            {
                ConfigSqlSugar(option.ConfigureSqlSugarAction);
            }

            //添加跨域服务
            if (option.CorsOption.SetupCors is not null)
            {
                AddCors(builder, option.CorsOption.SetupCors);
            }

            //添加认证授权服务
            AddAuth(builder, option.AuthOption);

            //接口入参验证
            if (option.IsUseParamValidate)
            {
                UseParamValidate(builder);
            }

            //响应缓存
            if (option.IsUseResponseCaching)
            {
                AddResponseCaching(builder);
            }

            //压缩
            AddCompression(builder, option.CompressionOption);

            #region swagger

            switch (option.SwaggerOption.EnmSwagger)
            {
                case TdbEnmSwagger.None:
                    TdbLogger.Ins.Info("不应用swagger");
                    break;
                case TdbEnmSwagger.Only:
                case TdbEnmSwagger.ApiVer:
                    AddSwagger(builder, option.SwaggerOption);
                    break;
                default:
                    throw new TdbException("不支持的swagger选项");
            }

            #endregion

            //注入请求上下文
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //httpclient
            builder.Services.AddTdbHttpClient();

            //控制器
            var mcvBuilder = builder.Services.AddControllers(option =>
            {
                //通用异常处理
                option.AddTdbGlobalExceptionFilter();
            });

            //json
            if (option.ConfigureJsonAction is not null)
            {
                ConfigureJson(mcvBuilder, option.ConfigureJsonAction);
            }

            //创建web应用
            var app = builder.Build();

            //ioc
            TdbIOC.Init(app.Services);

            //跨域服务
            if (option.CorsOption.UseCors is not null)
            {
                UseCors(app, option.CorsOption.UseCors);
            }

            //认证授权服务
            UseAuth(app);

            //响应缓存
            if (option.IsUseResponseCaching)
            {
                UseResponseCaching(app);
            }

            //压缩
            UseCompression(app, option.CompressionOption);

            #region swagger

            switch (option.SwaggerOption.EnmSwagger)
            {
                case TdbEnmSwagger.Only:
                case TdbEnmSwagger.ApiVer:
                    UseSwagger(app, option.SwaggerOption);
                    break;
            }

            #endregion

            app.MapControllers();

            app.Run();
        }

        /// <summary>
        /// 使用nlog日志服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="option">nlog配置</param>
        private static void UseNlog(WebApplicationBuilder builder, TdbNLogOption option)
        {
            //日志
            builder.Services.AddTdbNLogger(option.ConfigFile);

            //打日志
            TdbLogger.Ins.Info("应用nlog日志");
        }

        /// <summary>
        /// 使用autofac
        /// </summary>
        /// <param name="builder">A builder for web applications and services.</param>
        /// <param name="option">autofac选项</param>
        private static void UseAutofac(WebApplicationBuilder builder, TdbAutofacOption option)
        {
            //autofac容器注册
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                //autofac注册模块
                builder.RegisterModule(option.Module);
            });

            //打日志
            TdbLogger.Ins.Info("应用autofac容器");
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="initConfigAction">初始化配置方法</param>
        private static void InitConfig(Action initConfigAction)
        {
            initConfigAction();

            //打日志
            TdbLogger.Ins.Info("初始化配置");
        }

        /// <summary>
        /// 初始化hash id
        /// </summary>
        /// <param name="configureHashIDAction">配置hash id的方法</param>
        private static void InitHashID(Action<TdbHashIDOption> configureHashIDAction)
        {
            var option = new TdbHashIDOption();
            configureHashIDAction(option);

            TdbHashID.Init(option.Salt, minHashLength: option.MinHashLength, option.Alphabet, option.Seps);

            //打日志
            TdbLogger.Ins.Info("应用hash id");
        }

        /// <summary>
        /// 使用内存缓存服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setOption">设置内存缓存配置的方法</param>
        private static void UseMemoryCache(WebApplicationBuilder builder, Action<MemoryCacheOptions>? setOption)
        {
            var option = new MemoryCacheOptions();
            if (setOption is not null)
            {
                setOption(option);
            }

            builder.Services.AddTdbMemoryCache(option);

            //打日志
            TdbLogger.Ins.Info("应用内存缓存");
        }

        /// <summary>
        /// 使用redis缓存服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setOption">设置redis缓存配置的方法</param>
        private static void UseRedisCache(WebApplicationBuilder builder, Action<TdbRedisOption> setOption)
        {
            var option = new TdbRedisOption();
            setOption(option);

            if (option.ConnectionStrings is null || option.ConnectionStrings.Count == 0)
            {
                throw new TdbException("使用redis缓存时，redis连接字符串选项不能为空");
            }

            builder.Services.AddTdbRedisCache(option.ConnectionStrings.ToArray());

            //打日志
            TdbLogger.Ins.Info("应用redis缓存");
        }

        /// <summary>
        /// 使用MediatR
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="option">MediatR配置</param>
        private static void UseMediatR(WebApplicationBuilder builder, TdbMediatROption option)
        {
            builder.Services.AddTdbBusMediatR(() => option.RegisterAssemblys);

            //打日志
            TdbLogger.Ins.Info("应用MediatR");
        }

        /// <summary>
        /// 使用SqlSugar
        /// </summary>
        /// <param name="configSqlSugarAction">配置SqlSugar的方法</param>
        private static void ConfigSqlSugar(Action configSqlSugarAction)
        {
            configSqlSugarAction();

            //打日志
            TdbLogger.Ins.Info("应用SqlSugar");
        }

        /// <summary>
        /// 添加跨域服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setupCors">跨域请求选项设置方法</param>
        private static void AddCors(WebApplicationBuilder builder, Action<CorsOptions> setupCors)
        {
            builder.Services.AddCors(o => setupCors(o));

            //打日志
            TdbLogger.Ins.Info("添加跨域");
        }

        /// <summary>
        /// 使用跨域服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="useCors">应用跨域的方法</param>
        private static void UseCors(IApplicationBuilder app, Func<IApplicationBuilder, IApplicationBuilder> useCors)
        {
            useCors(app);

            //打日志
            TdbLogger.Ins.Info("应用跨域服务");
        }

        /// <summary>
        /// 添加认证授权服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="option">认证授权选项</param>
        private static void AddAuth(WebApplicationBuilder builder, TdbAuthOption option)
        {
            #region 认证

            var authBuilder = builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            if (option.SetupJwtBearer is not null)
            {
                authBuilder.AddJwtBearer(o =>
                {
                    option.SetupJwtBearer(o);
                });

                //打日志
                TdbLogger.Ins.Info("应用JwtBearer认证");
            }

            #endregion

            #region 授权

            builder.Services.AddAuthorization(o =>
            {
                if (option.WhiteListIP?.Count > 0)
                {
                    //要求接口调用方IP为白名单IP
                    o.AddTdbAuthWhiteListIP(option.WhiteListIP);

                    //打日志
                    TdbLogger.Ins.Info("应用白名单授权");
                }
            });

            #endregion
        }

        /// <summary>
        /// 应用认证授权服务
        /// </summary>
        /// <param name="app"></param>
        private static void UseAuth(IApplicationBuilder app)
        {
            //认证
            app.UseAuthentication();

            //授权
            app.UseAuthorization();

            //打日志
            TdbLogger.Ins.Info("应用认证授权服务");
        }

        /// <summary>
        /// 使用接口入参验证
        /// </summary>
        /// <param name="builder"></param>
        private static void UseParamValidate(WebApplicationBuilder builder)
        {
            builder.Services.AddTdbParamValidate();

            //打日志
            TdbLogger.Ins.Info("应用接口入参验证");
        }

        /// <summary>
        /// 添加响应缓存服务
        /// </summary>
        /// <param name="builder"></param>
        private static void AddResponseCaching(WebApplicationBuilder builder)
        {
            //响应缓存
            builder.Services.AddResponseCaching();

            //打日志
            TdbLogger.Ins.Info("添加响应缓存服务");
        }

        /// <summary>
        /// 应用响应缓存服务
        /// </summary>
        /// <param name="app"></param>
        private static void UseResponseCaching(IApplicationBuilder app)
        {
            //使用接口压缩
            app.UseResponseCaching();

            //打日志
            TdbLogger.Ins.Info("应用响应缓存服务");
        }

        /// <summary>
        /// 添加压缩服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="compressionOption">压缩选项</param>
        private static void AddCompression(WebApplicationBuilder builder, TdbCompressionOption compressionOption)
        {
            if (compressionOption.ConfigureCompressionOptions is not null)
            {
                //配置压缩选项
                builder.Services.AddResponseCompression(compressionOption.ConfigureCompressionOptions);

                if (compressionOption.ConfigureProvider is not null)
                {
                    //配置压缩算法Provider
                    compressionOption.ConfigureProvider(builder.Services);
                }

                //打日志
                TdbLogger.Ins.Info("添加压缩服务");
            }
        }

        /// <summary>
        /// 应用压缩服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="compressionOption">压缩选项</param>
        private static void UseCompression(IApplicationBuilder app, TdbCompressionOption compressionOption)
        {
            if (compressionOption.ConfigureCompressionOptions is not null)
            {
                //使用接口压缩
                app.UseResponseCompression();

                //打日志
                TdbLogger.Ins.Info("应用压缩服务");
            }
        }

        /// <summary>
        /// 添加swagger服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="option">swagger选项</param>
        private static void AddSwagger(WebApplicationBuilder builder, TdbSwaggerOption option)
        {
            if (option.SetupSwagger is null)
            {
                throw new TdbException("使用swagger时，SetupSwagger选项设置方法不能为空");
            }

            switch (option.EnmSwagger)
            {
                case TdbEnmSwagger.Only:
                    {
                        builder.Services.AddTdbSwaggerGen(o => option.SetupSwagger(o));

                        //打日志
                        TdbLogger.Ins.Info("添加swagger服务");
                    }
                    break;
                case TdbEnmSwagger.ApiVer:
                    {
                        //添加api版本控制及浏览服务
                        builder.Services.AddTdbApiVersionExplorer();

                        //添加Swagger服务（api版本管理）
                        builder.Services.AddTdbSwaggerGenApiVer(o => option.SetupSwagger(o));

                        //打日志
                        TdbLogger.Ins.Info("添加swagger及api版本控制服务");
                    }
                    break;
            }
        }

        /// <summary>
        /// 应用swagger服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="option">swagger选项</param>
        private static void UseSwagger(IApplicationBuilder app, TdbSwaggerOption option)
        {
            if (option.SetupSwagger is null)
            {
                throw new TdbException("使用swagger时，SetupSwagger选项设置方法不能为空");
            }

            switch (option.EnmSwagger)
            {
                case TdbEnmSwagger.Only:
                    {
                        app.UseTdbSwaggerAndUI(option.RoutePrefix);

                        //打日志
                        TdbLogger.Ins.Info("应用swagger服务");
                    }
                    break;
                case TdbEnmSwagger.ApiVer:
                    {
                        app.UseTdbSwaggerAndUIApiVer(option.RoutePrefix);

                        //打日志
                        TdbLogger.Ins.Info("应用swagger及api版本控制服务");
                    }
                    break;
            }
        }

        /// <summary>
        /// json配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureJsonAction">json选项配置方法</param>
        private static void ConfigureJson(IMvcBuilder builder, Action<Microsoft.AspNetCore.Mvc.JsonOptions> configureJsonAction)
        {
            builder.AddJsonOptions(configureJsonAction);

            //打日志
            TdbLogger.Ins.Info("配置json选项");
        }
    }

    /// <summary>
    /// webapi 服务及使用选项
    /// </summary>
    public class TdbWebAppBuilderOption
    {
        /// <summary>
        /// 日志选项
        /// </summary>
        public TdbLogOption LogOption { get; set; } = new TdbLogOption() { EnmLog = TdbEnmLog.NLog, NLogOption = new TdbNLogOption() };

        /// <summary>
        /// IOC选项
        /// </summary>
        public TdbIOCOption IOCOption { get; set; } = new TdbIOCOption() { EnmIOC = TdbEnmIOC.Autofac, AutofacOption = new TdbAutofacOption() };
        
        /// <summary>
        /// 初始化配置方法
        /// </summary>
        public Action? InitConfigAction { get; set; }

        /// <summary>
        /// 配置hash id的方法
        /// </summary>
        public Action<TdbHashIDOption>? ConfigureHashIDAction { get; set; }

        /// <summary>
        /// 缓存选项
        /// </summary>
        public TdbCacheOption CacheOption { get; set; } = new TdbCacheOption() { EnmCache = TdbEnmCache.Memory };

        /// <summary>
        /// 总线选项
        /// </summary>
        public TdbBusOption BusOption { get; set; } = new TdbBusOption();

        /// <summary>
        /// 配置SqlSugar的方法
        /// </summary>
        public Action? ConfigureSqlSugarAction { get; set; }

        /// <summary>
        /// 跨域请求选项
        /// </summary>
        public TdbCorsOption CorsOption { get; set; } = new TdbCorsOption();

        /// <summary>
        /// 认证授权选项
        /// </summary>
        public TdbAuthOption AuthOption { get; set; } = new TdbAuthOption();

        /// <summary>
        /// 是否启用接口入参验证
        /// </summary>
        public bool IsUseParamValidate { get; set; } = true;

        /// <summary>
        /// 是否使用响应缓存
        /// </summary>
        public bool IsUseResponseCaching { get; set; } = false;

        /// <summary>
        /// 压缩选项
        /// </summary>
        public TdbCompressionOption CompressionOption { get; set; } = new TdbCompressionOption();

        /// <summary>
        /// swagger选项
        /// </summary>
        public TdbSwaggerOption SwaggerOption { get; set; } = new TdbSwaggerOption();

        /// <summary>
        /// json选项配置方法
        /// </summary>
        public Action<Microsoft.AspNetCore.Mvc.JsonOptions>? ConfigureJsonAction { get; set; } = (o) =>
        {
            //以下序列化选项使用通用库中一样的设置
            o.JsonSerializerOptions.PropertyNamingPolicy = CvtHelper.DefaultOptions.PropertyNamingPolicy;
            o.JsonSerializerOptions.IncludeFields = CvtHelper.DefaultOptions.IncludeFields;
            o.JsonSerializerOptions.Encoder = CvtHelper.DefaultOptions.Encoder;
        };

        #region 内部定义

        #region 日志

        /// <summary>
        /// 日志类型枚举
        /// </summary>
        public enum TdbEnmLog
        {
            /// <summary>
            /// nlog
            /// </summary>
            NLog = 1
        }

        /// <summary>
        /// 日志选项
        /// </summary>
        public class TdbLogOption
        {
            /// <summary>
            /// 使用哪种日志组件
            /// </summary>
            public TdbEnmLog EnmLog { get; set; }

            /// <summary>
            /// 使用nlog日志时必须有值
            /// </summary>
            public TdbNLogOption? NLogOption { get; set; }
        }

        /// <summary>
        /// nlog日志配置
        /// </summary>
        public class TdbNLogOption
        {
            /// <summary>
            /// 配置文件名
            /// </summary>
            public string ConfigFile { get; set; } = "";
        }

        #endregion

        #region IOC

        /// <summary>
        /// ioc容器枚举
        /// </summary>
        public enum TdbEnmIOC
        {
            /// <summary>
            /// Autofac
            /// </summary>
            Autofac = 1
        }

        /// <summary>
        /// ioc选项
        /// </summary>
        public class TdbIOCOption
        {
            /// <summary>
            /// 使用哪种ioc组件
            /// </summary>
            public TdbEnmIOC EnmIOC { get; set; }

            /// <summary>
            /// 使用Autofac时必须有值
            /// </summary>
            public TdbAutofacOption? AutofacOption { get; set; }
        }

        /// <summary>
        /// autoface选项
        /// </summary>
        public class TdbAutofacOption
        {
            /// <summary>
            /// autofac注册模块（默认为：TdbAutofacModule）
            /// </summary>
            public IModule Module { get; set; } = new TdbAutofacModule();
        }

        #endregion

        #region hash id

        /// <summary>
        /// hash id 选项
        /// </summary>
        public class TdbHashIDOption
        {
            /// <summary>
            /// 盐
            /// </summary>
            public string Salt { get; set; } = "";

            /// <summary>
            /// 最小长度
            /// </summary>
            public int MinHashLength { get; set; } = 6;

            /// <summary>
            /// hash字母表
            /// </summary>
            public string Alphabet { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            /// <summary>
            /// 
            /// </summary>
            public string Seps { get; set; } = "cfhistuCFHISTU";
        }

        #endregion

        #region 缓存

        /// <summary>
        /// 缓存类型枚举
        /// </summary>
        public enum TdbEnmCache
        {
            /// <summary>
            /// 内存缓存
            /// </summary>
            Memory = 1,

            /// <summary>
            /// redis缓存
            /// </summary>
            Redis = 2
        }

        /// <summary>
        /// 缓存选项
        /// </summary>
        public class TdbCacheOption
        {
            /// <summary>
            /// 使用哪种缓存组件
            /// </summary>
            public TdbEnmCache EnmCache { get; set; }

            /// <summary>
            /// 设置内存缓存选项的方法
            /// </summary>
            public Action<MemoryCacheOptions>? ConfigureMemoryAction { get; set; }

            /// <summary>
            /// 设置redis缓存选项的方法
            /// </summary>
            public Action<TdbRedisOption>? ConfigureRedisAction { get; set; }
        }

        /// <summary>
        /// redis配置
        /// </summary>
        public class TdbRedisOption
        {
            /// <summary>
            /// 连接字符串集合
            /// </summary>
            public List<string>? ConnectionStrings { get; set; }
        }

        #endregion

        #region 总线

        /// <summary>
        /// 总线选项
        /// </summary>
        public class TdbBusOption
        {
            /// <summary>
            /// 是否启用MediatR
            /// </summary>
            public bool IsUseMediatR { get; set; }

            /// <summary>
            /// 启用MediatR时必须有值
            /// </summary>
            public TdbMediatROption? MediatROption { get; set; }
        }

        /// <summary>
        /// MediatR选项
        /// </summary>
        public class TdbMediatROption
        {
            /// <summary>
            /// 需注册的程序集集合
            /// </summary>
            public List<Assembly> RegisterAssemblys { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="registerAssemblys">需注册的程序集集合</param>
            public TdbMediatROption(List<Assembly>? registerAssemblys = null)
            {
                if (registerAssemblys is null || registerAssemblys.Count == 0)
                {
                    registerAssemblys = new TdbMediatRAssemblyModule().GetRegisterAssemblys();
                }

                this.RegisterAssemblys = registerAssemblys;
            }
        }

        #endregion

        #region 跨域

        /// <summary>
        /// 跨域请求选项
        /// </summary>
        public class TdbCorsOption
        {
            /// <summary>
            /// 跨域选项设置方法
            /// </summary>
            public Action<CorsOptions>? SetupCors { get; set; }

            /// <summary>
            /// 应用跨域的方法
            /// </summary>
            public Func<IApplicationBuilder, IApplicationBuilder>? UseCors { get; set; }

            /// <summary>
            /// 允许所有跨域请求
            /// </summary>
            /// <param name="options">跨域请求选项</param>
            /// <returns></returns>
            public void SetupCorsAllowAll(CorsOptions options)
            {
                options.AddPolicy(TdbCorsExtensions.AllAllowCorsPolicyName, builder =>
                {
                    builder.WithOrigins("urls")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(_ => true) // =AllowAnyOrigin()
                        .AllowCredentials();
                });
            }

            /// <summary>
            /// 允许所有跨域请求
            /// </summary>
            /// <param name="app">管道配置类</param>
            /// <returns></returns>
            public IApplicationBuilder UseCorsAllAllow(IApplicationBuilder app)
            {
                // 跨域设置
                return app.UseCors(TdbCorsExtensions.AllAllowCorsPolicyName);
            }
        }

        #endregion

        #region 认证授权

        /// <summary>
        /// 认证授权选项
        /// </summary>
        public class TdbAuthOption
        {
            /// <summary>
            /// ip白名单集合
            /// </summary>
            public List<string>? WhiteListIP { get; set; }

            /// <summary>
            /// 设置jwt bearer配置
            /// </summary>
            public Action<JwtBearerOptions>? SetupJwtBearer { get; set; }

            /// <summary>
            /// 使用默认的jwt bearer选项
            /// </summary>
            /// <param name="option">jwt bearer选项</param>
            /// <param name="getSecretKey">获取秘钥的方法</param>
            public void DefaultJwtBearerOptions(JwtBearerOptions option, Func<string> getSecretKey)
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = TdbClaimTypes.UName,
                    RoleClaimType = TdbClaimTypes.RoleID,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(getSecretKey())),
                    //不验Audience
                    ValidateAudience = false,
                    //不验Issuer
                    ValidateIssuer = false,
                    //允许的服务器时间偏移量
                    ClockSkew = TimeSpan.FromSeconds(10),

                    /***********************************TokenValidationParameters的参数默认值***********************************/
                    // RequireSignedTokens = true,
                    // SaveSigninToken = false,
                    // ValidateActor = false,
                    // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                    // ValidateAudience = true,
                    // ValidateIssuer = true, 
                    // ValidateIssuerSigningKey = false,
                    // 是否要求Token的Claims中必须包含Expires
                    // RequireExpirationTime = true,
                    // 允许的服务器时间偏移量
                    // ClockSkew = TimeSpan.FromSeconds(300),
                    // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                    // ValidateLifetime = true
                };
                option.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        TdbLogger.Ins.Log(EnmTdbLogLevel.Warn, context.Exception, "认证授权异常");
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/html;charset=utf-8";
                        context.Response.StatusCode = 403;
                        context.Response.WriteAsync("权限不足");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.Clear();
                        context.Response.ContentType = "text/html;charset=utf-8";
                        context.Response.StatusCode = 401;
                        context.Response.WriteAsync("认证未通过");
                        return Task.CompletedTask;
                    }
                };
            }
        }

        #endregion

        #region 压缩

        /// <summary>
        /// 压缩选项
        /// </summary>
        public class TdbCompressionOption
        {
            /// <summary>
            /// 配置压缩选项方法
            /// </summary>
            public Action<ResponseCompressionOptions>? ConfigureCompressionOptions { get; set; }

            /// <summary>
            /// 配置压缩算法Provider的方法
            /// </summary>
            public Action<IServiceCollection>? ConfigureProvider { get; set; }

            /// <summary>
            /// 默认的压缩选项配置方法
            /// </summary>
            /// <param name="options">压缩选项</param>
            public void DefaultConfigureCompressionOptions(ResponseCompressionOptions options)
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();

                //针对指定的MimeType来使用压缩策略
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
                options.EnableForHttps = true;
            }

            /// <summary>
            /// 默认的压缩算法Provider配置方法
            /// </summary>
            /// <param name="services"></param>
            public void DefaultConfigureProvider(IServiceCollection services)
            {
                //针对不同的压缩类型，设置对应的压缩级别
                services.Configure<BrotliCompressionProviderOptions>(options =>
                {
                    //使用最优的方式进行压缩，即使花费的时间较长
                    options.Level = CompressionLevel.Optimal;
                });
                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    //使用最优的方式进行压缩，即使花费的时间较长
                    options.Level = CompressionLevel.Optimal;
                });
            }
        }

        #endregion

        #region swagger

        /// <summary>
        /// swagger设置枚举
        /// </summary>
        public enum TdbEnmSwagger
        {
            /// <summary>
            /// 不使用swagger
            /// </summary>
            None = 1,

            /// <summary>
            /// 仅使用swagger
            /// </summary>
            Only = 2,

            /// <summary>
            /// 使用swagger且带api版本控制
            /// </summary>
            ApiVer = 3
        }

        /// <summary>
        /// swagger选项
        /// </summary>
        public class TdbSwaggerOption
        {
            /// <summary>
            /// swagger设置
            /// </summary>
            public TdbEnmSwagger EnmSwagger { get; set; }

            /// <summary>
            /// swagger选项设置方法
            /// </summary>
            public Action<TdbSwaggerOptions>? SetupSwagger { get; set; }

            /// <summary>
            /// swagger路由前缀
            /// </summary>
            public string RoutePrefix { get; set; } = "swagger";
        }

        #endregion

        #endregion
    }
}
