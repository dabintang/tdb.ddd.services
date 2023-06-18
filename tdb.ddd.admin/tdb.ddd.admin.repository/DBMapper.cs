using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.admin.domain.OperationRecord.Aggregate;
using tdb.ddd.admin.repository.DBEntity;

namespace tdb.ddd.admin.repository
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
                //操作记录实体映射
                OperationRecordMapping(cfg);
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
        /// 操作记录实体映射
        /// </summary>
        /// <param name="cfg">映射配置表达式</param>
        private static void OperationRecordMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<OperationRecordAgg, OperationRecord>();
        }
    }
}
