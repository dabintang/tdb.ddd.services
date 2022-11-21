using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.webapi;

namespace tdb.demo.webapi.Controllers.V1
{
    /// <summary>
    /// ioc
    /// </summary>
    [TdbApiVersion(1)]
    [AllowAnonymous]
    public class IocController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ITestIoc testIoc1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testIoc"></param>
        public IocController(ITestIoc testIoc)
        {
            this.testIoc1 = testIoc;
        }

        #region 接口

        /// <summary>
        /// 测试IOC
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public TdbRes<string> Get()
        {
            this.testIoc1.Age = 10;
            var testIoc2 = TdbIOC.GetService<ITestIoc>();
            TdbAssert.Equals(true, object.ReferenceEquals(testIoc1, testIoc2));

            return TdbRes.Success("OK");
        }

        #endregion

        #region 定义

        /// <summary>
        /// 
        /// </summary>
        public interface ITestIoc : ITdbIOCScoped
        {
            /// <summary>
            /// 
            /// </summary>
            int Age { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class TestIoc : ITestIoc
        {
            /// <summary>
            /// 
            /// </summary>
            public int Age { get; set; }
        }

        #endregion
    }
}
