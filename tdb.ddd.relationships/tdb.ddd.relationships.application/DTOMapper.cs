using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO.Circle;
using tdb.ddd.relationships.application.contracts.V1.DTO.Personnel;
using tdb.ddd.relationships.domain.Circle.Aggregate;
using tdb.ddd.relationships.domain.Personnel.Aggregate;

namespace tdb.ddd.relationships.application
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
                //人际圈实体映射
                CircleMapping(cfg);
                //人员实体映射
                PersonnelMapping(cfg);
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
        /// 人际圈实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void CircleMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<CircleAgg, GetCircleRes>()
               .ForMember(dest => dest.CreatorID, opts => opts.MapFrom(src => src.CreateInfo.CreatorID));
        }

        /// <summary>
        /// 人员实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void PersonnelMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PersonnelAgg, GetPersonnelRes>();
            cfg.CreateMap<PersonnelAgg, GetPersonnelByUserIDRes>();
        }
    }
}
