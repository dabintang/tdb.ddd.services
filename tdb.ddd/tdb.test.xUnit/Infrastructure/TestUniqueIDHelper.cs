using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.test.xUnit.Infrastructure
{
    /// <summary>
    /// 测试唯一编码生成器
    /// </summary>
    public class TestUniqueIDHelper
    {
        /// <summary>
        /// 测试连续生成3000个ID无重复
        /// </summary>
        [Fact]
        public void TestCreateID3000()
        {
            var list = new List<long>();
            for (int i = 0; i < 3000; i++)
            {
                list.Add(UniqueIDHelper.CreateID(0, 0));
            }

            Assert.True(list.Distinct().Count() == list.Count);
        }
    }
}
