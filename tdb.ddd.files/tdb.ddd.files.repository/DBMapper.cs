using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;
using tdb.ddd.files.domain.Files.Aggregate;
using tdb.ddd.files.repository.DBEntity;

namespace tdb.ddd.files.repository
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
                //临时文件实体映射
                TempFileMapping(cfg);
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
            return Mapper.Map(source, destination);
        }

        /// <summary>
        /// 临时文件实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void TempFileMapping(IMapperConfigurationExpression cfg)
        {
#pragma warning disable CS8602 // 解引用可能出现空引用。

            cfg.CreateMap<FileAgg, DBEntity.FileInfo>()
                .ForMember(dest => dest.CreatorID, opts => opts.MapFrom(src => src.CreateInfo.CreatorID))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateInfo.CreateTime))
                .ForMember(dest => dest.UpdaterID, opts => opts.MapFrom(src => src.UpdateInfo.UpdaterID))
                .ForMember(dest => dest.UpdateTime, opts => opts.MapFrom(src => src.UpdateInfo.UpdateTime));
            cfg.CreateMap<DBEntity.FileInfo, FileAgg>()
                .ForMember(dest => dest.CreateInfo, opts => opts.MapFrom(src => new CreateInfoValueObject() { CreatorID = src.CreatorID, CreateTime = src.CreateTime }))
                .ForMember(dest => dest.UpdateInfo, opts => opts.MapFrom(src => new UpdateInfoValueObject() { UpdaterID = src.UpdaterID, UpdateTime = src.UpdateTime }));

#pragma warning restore CS8602 // 解引用可能出现空引用。
        }
    }
}
