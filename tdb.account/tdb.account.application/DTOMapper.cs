﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.account.application.contracts.V1.DTO;
using tdb.account.domain.contracts.User;
using tdb.account.domain.User.Aggregate;
using tdb.ddd.contracts;

namespace tdb.account.application
{
    /// <summary>
    /// 传输对象与聚合实体间映射器
    /// </summary>
    public class DTOMapper
    {
        /// <summary>
        /// 映射器
        /// </summary>
        private static IMapper Mapper { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        static DTOMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
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
        /// 用户实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void UserMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<UserAgg, UserInfoRes>()
               .ForMember(dest => dest.MobilePhone, opts => opts.MapFrom(src => src.MobilePhoneValue.MobilePhone ?? ""))
               .ForMember(dest => dest.IsMobilePhoneVerified, opts => opts.MapFrom(src => src.MobilePhoneValue.IsMobilePhoneVerified))
               .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.EmailValue.Email ?? ""))
               .ForMember(dest => dest.IsEmailVerified, opts => opts.MapFrom(src => src.EmailValue.IsEmailVerified))
               .ForMember(dest => dest.CreatorID, opts => opts.MapFrom(src => src.CreateInfo.CreatorID))
               .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateInfo.CreateTime))
               .ForMember(dest => dest.UpdaterID, opts => opts.MapFrom(src => src.UpdateInfo.UpdaterID))
               .ForMember(dest => dest.UpdateTime, opts => opts.MapFrom(src => src.UpdateInfo.UpdateTime));

            cfg.CreateMap<UserLoginResult, UserLoginRes>();
            cfg.CreateMap<TdbRes<UserLoginResult>, TdbRes<UserLoginRes>>();
        }
    }
}
