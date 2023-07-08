using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;
using tdb.ddd.relationships.domain.Circle.Aggregate;
using tdb.ddd.relationships.domain.Personnel.Aggregate;
using tdb.ddd.relationships.domain.Photo.Aggregate;
using tdb.ddd.relationships.repository.DBEntity;

namespace tdb.ddd.relationships.repository
{
    /// <summary>
    /// 数据库实体与聚合实体间映射器
    /// </summary>
    public class DBMapper
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
                //照片实体映射
                PhotoMapping(cfg);
                //人员实体映射
                PersonnelMapping(cfg);
                //人际圈实体映射
                CircleMapping(cfg);
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
        /// 照片实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void PhotoMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PhotoAgg, PhotoInfo>()
                .ForMember(dest => dest.CreatorID, opts => opts.MapFrom(src => src.CreateInfo.CreatorID))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateInfo.CreateTime));
            cfg.CreateMap<PhotoInfo, PhotoAgg>()
                .ForMember(dest => dest.CreateInfo, opts => opts.MapFrom(src => new CreateInfoValueObject() { CreatorID = src.CreatorID, CreateTime = src.CreateTime }));
        }

        /// <summary>
        /// 人员实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void PersonnelMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PersonnelAgg, PersonnelInfo>()
                .ForMember(dest => dest.CreatorID, opts => opts.MapFrom(src => src.CreateInfo.CreatorID))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateInfo.CreateTime))
                .ForMember(dest => dest.UpdaterID, opts => opts.MapFrom(src => src.UpdateInfo.UpdaterID))
                .ForMember(dest => dest.UpdateTime, opts => opts.MapFrom(src => src.UpdateInfo.UpdateTime));
            cfg.CreateMap<PersonnelInfo, PersonnelAgg>()
                .ForMember(dest => dest.CreateInfo, opts => opts.MapFrom(src => new CreateInfoValueObject() { CreatorID = src.CreatorID, CreateTime = src.CreateTime }))
                .ForMember(dest => dest.UpdateInfo, opts => opts.MapFrom(src => new UpdateInfoValueObject() { UpdaterID = src.UpdaterID, UpdateTime = src.UpdateTime }));
        }

        /// <summary>
        /// 人际圈实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void CircleMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<CircleAgg, CircleInfo>()
                .ForMember(dest => dest.CreatorID, opts => opts.MapFrom(src => src.CreateInfo.CreatorID))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateInfo.CreateTime))
                .ForMember(dest => dest.UpdaterID, opts => opts.MapFrom(src => src.UpdateInfo.UpdaterID))
                .ForMember(dest => dest.UpdateTime, opts => opts.MapFrom(src => src.UpdateInfo.UpdateTime));
            cfg.CreateMap<CircleInfo, CircleAgg>()
                .ForMember(dest => dest.CreateInfo, opts => opts.MapFrom(src => new CreateInfoValueObject() { CreatorID = src.CreatorID, CreateTime = src.CreateTime }))
                .ForMember(dest => dest.UpdateInfo, opts => opts.MapFrom(src => new UpdateInfoValueObject() { UpdaterID = src.UpdaterID, UpdateTime = src.UpdateTime }));
            cfg.CreateMap<MemberEntity, CircleMemberInfo>()
                .ForMember(dest => dest.CreatorID, opts => opts.MapFrom(src => src.CreateInfo.CreatorID))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateInfo.CreateTime));
            cfg.CreateMap<CircleMemberInfo, MemberEntity>()
                .ForMember(dest => dest.CreateInfo, opts => opts.MapFrom(src => new CreateInfoValueObject() { CreatorID = src.CreatorID, CreateTime = src.CreateTime }));
        }
    }
}
