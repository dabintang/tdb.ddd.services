using System.Reflection;
using tdb.ddd.infrastructure.Services;

namespace tdb.demo.webapi
{
    /// <summary>
    /// autofac注册模块
    /// </summary>
    public class AutofacModuleRegister : TdbAutofacModule
    {
        /// <summary>
        /// 获取需要注册的程序集集合
        /// </summary>
        /// <returns></returns>
        protected override List<Assembly> GetRegisterAssemblys()
        {
            var list = new List<Assembly>
            {
                Assembly.GetExecutingAssembly(),
                Assembly.Load("tdb.ddd.webapi")
        };

            return list;
        }
    }
}
