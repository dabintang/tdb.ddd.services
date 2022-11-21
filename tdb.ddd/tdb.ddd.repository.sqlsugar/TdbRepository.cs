using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.repository.sqlsugar
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="T">表实体类型</typeparam>
    public class TdbRepository<T> : SimpleClient<T> where T : class, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">上下文</param>
        public TdbRepository(ISqlSugarClient? context = null) : base(context)//注意这里要有默认值等于null，不然无参方式 IOC 不好注入
        {
            //base.Context = context;//ioc注入的对象
            //base.Context = DbScoped.SugarScope; //SqlSugar.Ioc这样写
            // base.Context=DbHelper.GetDbInstance();//当然也可以手动去赋值

            base.Context = TdbDbScoped.GetScopedContext();
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTran()
        {
            this.AsTenant().BeginTran();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            this.AsTenant().CommitTran();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            this.AsTenant().RollbackTran();
        }
    }
}
