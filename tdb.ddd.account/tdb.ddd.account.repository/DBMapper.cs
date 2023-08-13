using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.domain.Authority.Aggregate;
using tdb.ddd.account.domain.Certificate.Aggregate;
using tdb.ddd.account.domain.Role.Aggregate;
using tdb.ddd.account.domain.User.Aggregate;
using tdb.ddd.account.repository.DBEntity;
using tdb.ddd.domain;

namespace tdb.ddd.account.repository
{
    /// <summary>
    /// 数据库实体与聚合实体间映射器
    /// </summary>
    public static class DBMapper
    {
        /// <summary>
        /// 映射器
        /// </summary>
        private static IMapper Mapper { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        static DBMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                //权限实体映射
                AuthorityMapping(cfg);
                //角色实体映射
                RoleMapping(cfg);
                //用户实体映射
                UserMapping(cfg);
            });
            Mapper = configuration.CreateMapper();//或者var mapper=new Mapper(configuration);
        }

        /// <summary>
        /// 映射到新目标对象
        /// </summary>
        /// <typeparam name="TSource">源对象类型</typeparam>
        /// <typeparam name="TDestination">目标对象类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns></returns>
        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// 映射到传入的目标对象
        /// </summary>
        /// <typeparam name="TSource">源对象类型</typeparam>
        /// <typeparam name="TDestination">目标对象类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        /// <returns></returns>
        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map<TSource, TDestination>(source, destination);
        }

        /// <summary>
        /// 权限实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void AuthorityMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<AuthorityAgg, AuthorityInfo>();
            cfg.CreateMap<AuthorityInfo, AuthorityAgg>();
        }

        /// <summary>
        /// 角色实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void RoleMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<RoleAgg, RoleInfo>();
            cfg.CreateMap<RoleInfo, RoleAgg>();
        }

        /// <summary>
        /// 用户实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void UserMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<UserAgg, UserInfo>()
                .ForMember(dest => dest.MobilePhone, opts => opts.MapFrom(src => src.MobilePhoneValue.MobilePhone ?? ""))
                .ForMember(dest => dest.IsMobilePhoneVerified, opts => opts.MapFrom(src => src.MobilePhoneValue.IsMobilePhoneVerified))
                .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.EmailValue.Email ?? ""))
                .ForMember(dest => dest.IsEmailVerified, opts => opts.MapFrom(src => src.EmailValue.IsEmailVerified))
                .ForMember(dest => dest.CreatorID, opts => opts.MapFrom(src => src.CreateInfo.CreatorID))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateInfo.CreateTime))
                .ForMember(dest => dest.UpdaterID, opts => opts.MapFrom(src => src.UpdateInfo.UpdaterID))
                .ForMember(dest => dest.UpdateTime, opts => opts.MapFrom(src => src.UpdateInfo.UpdateTime));
            cfg.CreateMap<UserInfo, UserAgg>()
                .ForMember(dest => dest.MobilePhoneValue, opts => opts.MapFrom(src => new MobilePhoneValueObject() { MobilePhone = src.MobilePhone, IsMobilePhoneVerified = src.IsMobilePhoneVerified }))
                .ForMember(dest => dest.EmailValue, opts => opts.MapFrom(src => new EmailValueObject() { Email = src.Email, IsEmailVerified = src.IsEmailVerified }))
                .ForMember(dest => dest.CreateInfo, opts => opts.MapFrom(src => new CreateInfoValueObject() { CreatorID = src.CreatorID, CreateTime = src.CreateTime }))
                .ForMember(dest => dest.UpdateInfo, opts => opts.MapFrom(src => new UpdateInfoValueObject() { UpdaterID = src.UpdaterID, UpdateTime = src.UpdateTime }));

            cfg.CreateMap<CertificateAgg, CertificateInfo>()
              .ForMember(dest => dest.CreatorID, opts => opts.MapFrom(src => src.CreateInfo.CreatorID))
              .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateInfo.CreateTime));
            cfg.CreateMap<CertificateInfo, CertificateAgg>()
                 .ForMember(dest => dest.CreateInfo, opts => opts.MapFrom(src => new CreateInfoValueObject() { CreatorID = src.CreatorID, CreateTime = src.CreateTime }));

        }
    }
}
