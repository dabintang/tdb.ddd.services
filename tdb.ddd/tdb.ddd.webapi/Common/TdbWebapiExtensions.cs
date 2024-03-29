﻿using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using tdb.common;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Services;

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
                case TdbWebAppBuilderOption.TdbEnmLog.NLog:
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
                case TdbWebAppBuilderOption.TdbEnmIOC.Autofac:
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
            if (option.SetupHashID is not null)
            {
                InitHashID(option.SetupHashID);
            }

            #region 缓存

            switch (option.CacheOption.EnmCache)
            {
                case TdbWebAppBuilderOption.TdbEnmCache.Memory:
                    {
                        //使用内存缓存服务
                        UseMemoryCache(builder, option.CacheOption.SetupMemory);
                    }
                    break;
                case TdbWebAppBuilderOption.TdbEnmCache.Redis:
                    {
                        if (option.CacheOption.SetupRedis is null)
                        {
                            throw new TdbException("使用redis缓存时，RedisOption选项不能为空");
                        }

                        //使用redis缓存
                        UseRedisCache(builder, option.CacheOption.SetupRedis);
                    }
                    break;
                default:
                    throw new TdbException("不支持的缓存类型");
            }

            #endregion

            #region 总线

            if (option.BusOption.MediatROption is not null)
            {
                UseMediatR(builder, option.BusOption.MediatROption);
            }

            if (option.BusOption.CAPOption is not null)
            {
                UseDotNetCoreCAP(builder, option.BusOption.CAPOption);
            }

            #endregion

            //SqlSugar
            if (option.SetupSqlSugar is not null)
            {
                ConfigSqlSugar(option.SetupSqlSugar);
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
                case TdbWebAppBuilderOption.TdbEnmSwagger.None:
                    TdbLogger.Ins.Info("不应用swagger");
                    break;
                case TdbWebAppBuilderOption.TdbEnmSwagger.Only:
                case TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer:
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
            if (option.SetupJson is not null)
            {
                SetupJson(mcvBuilder, option.SetupJson);
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
                case TdbWebAppBuilderOption.TdbEnmSwagger.Only:
                case TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer:
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
        private static void UseNlog(WebApplicationBuilder builder, TdbWebAppBuilderOption.TdbNLogOption option)
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
        private static void UseAutofac(WebApplicationBuilder builder, TdbWebAppBuilderOption.TdbAutofacOption option)
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
        /// <param name="setupHashID">配置hash id的方法</param>
        private static void InitHashID(Action<TdbWebAppBuilderOption.TdbHashIDOption> setupHashID)
        {
            var option = new TdbWebAppBuilderOption.TdbHashIDOption();
            setupHashID(option);

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
        private static void UseRedisCache(WebApplicationBuilder builder, Action<TdbWebAppBuilderOption.TdbRedisOption> setOption)
        {
            var option = new TdbWebAppBuilderOption.TdbRedisOption();
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
        private static void UseMediatR(WebApplicationBuilder builder, TdbWebAppBuilderOption.TdbMediatROption option)
        {
            builder.Services.AddTdbBusMediatR(option.AssemblyModule);

            //打日志
            TdbLogger.Ins.Info("应用MediatR");
        }

        /// <summary>
        /// 使用DotNetCore.CAP
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="capOption">DotNetCore.CAP选项</param>
        private static void UseDotNetCoreCAP(WebApplicationBuilder builder, TdbWebAppBuilderOption.TdbDotNetCoreCAPOption capOption)
        {
            builder.Services.AddTdbBusCAP(capOption.SetupDotNetCoreCAP, capOption.AssemblyModule);

            //打日志
            TdbLogger.Ins.Info("应用DotNetCore.CAP");
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
        private static void AddAuth(WebApplicationBuilder builder, TdbWebAppBuilderOption.TdbAuthOption option)
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
                if (option.GetWhiteListIP is not null)
                {
                    var lstWhiteListIP = option.GetWhiteListIP();
                    //要求接口调用方IP为白名单IP
                    o.AddTdbAuthWhiteListIP(lstWhiteListIP);

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
        private static void AddCompression(WebApplicationBuilder builder, TdbWebAppBuilderOption.TdbCompressionOption compressionOption)
        {
            if (compressionOption.SetupCompression is not null)
            {
                //配置压缩选项
                builder.Services.AddResponseCompression(compressionOption.SetupCompression);

                if (compressionOption.SetupProvider is not null)
                {
                    //配置压缩算法Provider
                    compressionOption.SetupProvider(builder.Services);
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
        private static void UseCompression(IApplicationBuilder app, TdbWebAppBuilderOption.TdbCompressionOption compressionOption)
        {
            if (compressionOption.SetupCompression is not null)
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
        private static void AddSwagger(WebApplicationBuilder builder, TdbWebAppBuilderOption.TdbSwaggerOption option)
        {
            if (option.SetupSwagger is null)
            {
                throw new TdbException("使用swagger时，SetupSwagger选项设置方法不能为空");
            }

            switch (option.EnmSwagger)
            {
                case TdbWebAppBuilderOption.TdbEnmSwagger.Only:
                    {
                        builder.Services.AddTdbSwaggerGen(o => option.SetupSwagger(o));

                        //打日志
                        TdbLogger.Ins.Info("添加swagger服务");
                    }
                    break;
                case TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer:
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
        private static void UseSwagger(IApplicationBuilder app, TdbWebAppBuilderOption.TdbSwaggerOption option)
        {
            if (option.SetupSwagger is null)
            {
                throw new TdbException("使用swagger时，SetupSwagger选项设置方法不能为空");
            }

            switch (option.EnmSwagger)
            {
                case TdbWebAppBuilderOption.TdbEnmSwagger.Only:
                    {
                        app.UseTdbSwaggerAndUI(option.RoutePrefix);

                        //打日志
                        TdbLogger.Ins.Info("应用swagger服务");
                    }
                    break;
                case TdbWebAppBuilderOption.TdbEnmSwagger.ApiVer:
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
        /// <param name="setupJson">json选项配置方法</param>
        private static void SetupJson(IMvcBuilder builder, Action<Microsoft.AspNetCore.Mvc.JsonOptions> setupJson)
        {
            builder.AddJsonOptions(setupJson);

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
        public Action<TdbHashIDOption>? SetupHashID { get; set; }

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
        public Action? SetupSqlSugar { get; set; }

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
        public Action<Microsoft.AspNetCore.Mvc.JsonOptions>? SetupJson { get; set; } = (o) =>
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
            public Action<MemoryCacheOptions>? SetupMemory { get; set; }

            /// <summary>
            /// 设置redis缓存选项的方法
            /// </summary>
            public Action<TdbRedisOption>? SetupRedis { get; set; }
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
            /// 启用MediatR时必须有值
            /// </summary>
            public TdbMediatROption? MediatROption { get; set; }

            /// <summary>
            /// 启用DotNetCore.CAP时必须有值
            /// </summary>
            public TdbDotNetCoreCAPOption? CAPOption { get; set; }
        }

        /// <summary>
        /// MediatR选项
        /// </summary>
        public class TdbMediatROption
        {
            /// <summary>
            /// 获取需注册程序集
            /// </summary>
            public TdbBusAssemblyModule AssemblyModule { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="assemblyModule">获取需注册程序集</param>
            public TdbMediatROption(TdbBusAssemblyModule ? assemblyModule = null)
            {
                this.AssemblyModule = assemblyModule ?? new TdbBusAssemblyModule();
            }
        }

        /// <summary>
        /// DotNetCore.CAP选项
        /// </summary>
        public class TdbDotNetCoreCAPOption
        {
            /// <summary>
            /// 设置DotNetCore.CAP选项的方法
            /// </summary>
            public Action<CapOptions> SetupDotNetCoreCAP { get; set; }

            /// <summary>
            /// 获取需注册程序集
            /// </summary>
            public TdbBusAssemblyModule AssemblyModule { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="setupDotNetCoreCAP">设置DotNetCore.CAP选项的方法</param>
            /// <param name="assemblyModule">获取需注册程序集</param>
            public TdbDotNetCoreCAPOption(Action<CapOptions> setupDotNetCoreCAP, TdbBusAssemblyModule? assemblyModule = null)
            {
                this.AssemblyModule = assemblyModule ?? new TdbBusAssemblyModule();
                this.SetupDotNetCoreCAP = (o) =>
                {
                    DefaultCapOptionsWithoutTransportAndStorage(o);
                    setupDotNetCoreCAP(o);
                };
            }

            /// <summary>
            /// 设置DotNetCore.CAP默认选项，Transport和Storage除外。
            /// </summary>
            /// <param name="options">DotNetCore.CAP选项</param>
            public void DefaultCapOptionsWithoutTransportAndStorage(CapOptions options)
            {
                //默认分组名称
                options.DefaultGroupName = "default";
                //分组名称前缀
                options.GroupNamePrefix = "tdb.ddd.group";
                //主题名称前缀
                options.TopicNamePrefix = "tdb.ddd.topic";
                ////版本号
                //options.Version = "v1";
                ////成功消息过期时间（秒数）
                //options.SucceedMessageExpiredAfter = 24 * 3600;
                //已失败消息过期时间（秒数）
                options.FailedMessageExpiredAfter = 30 * 24 * 3600;
                ////投递/处理失败的消息重试间隔（秒数）
                //options.FailedRetryInterval = 60;
                ////重试失败后回调函数
                //options.FailedThresholdCallback = null;
                ////重试次数
                //options.FailedRetryCount = 50;
                //消费者线程数，如果值大于1则不保证消息的执行顺序
                options.ConsumerThreadCount = 10;
                //是否每组独立调度【[Obsolete("Use EnableConsumerPrefetch instead. Setting it to true means that each consumer is now executed concurrently by thread pool, regardless of whether they are in different groups.")]】
                //options.UseDispatchingPerGroup = true;
                //启用消费者预取
                options.EnableConsumerPrefetch = true;
                //生产者线程数，如值大于1则不保证消息的执行顺序
                //options.ProducerThreadCount = 1;
                //json序列号选项
                options.JsonSerializerOptions.IncludeFields = CvtHelper.DefaultOptions.IncludeFields;
                //删除过期消息间隔（秒数）
                options.CollectorCleaningInterval = 3600;

                //仪表板
                options.UseDashboard();
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
            /// 获取ip白名单的方法
            /// </summary>
            public Func<List<string>>? GetWhiteListIP { get; set; }

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
            public Action<ResponseCompressionOptions>? SetupCompression { get; set; }

            /// <summary>
            /// 配置压缩算法Provider的方法
            /// </summary>
            public Action<IServiceCollection>? SetupProvider { get; set; }

            /// <summary>
            /// 默认的压缩选项配置方法
            /// </summary>
            /// <param name="options">压缩选项</param>
            public void DefaultSetupCompression(ResponseCompressionOptions options)
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
            public void DefaultSetupProvider(IServiceCollection services)
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
