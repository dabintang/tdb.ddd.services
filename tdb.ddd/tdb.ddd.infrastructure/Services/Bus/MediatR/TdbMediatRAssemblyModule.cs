using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace tdb.ddd.infrastructure.Services
{
    /// <summary>
    /// 用到MediatR的程序集模块
    /// </summary>
    public class TdbMediatRAssemblyModule
    {
        /// <summary>
        /// 获取需要注册的程序集名称集合
        /// （默认获取所有tdb.开头 .dll结尾的程序集：tdb.*.dll）
        /// </summary>
        /// <returns></returns>
        public virtual List<Assembly> GetRegisterAssemblys()
        {
            var list = new List<Assembly>();
            var dllFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "tdb.*.dll", SearchOption.AllDirectories);
            foreach (var dllFileName in dllFileNames)
            {
                list.Add(Assembly.LoadFrom(dllFileName));
            }

            return list;
        }
    }
}
