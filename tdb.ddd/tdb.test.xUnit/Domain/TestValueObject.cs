using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;

namespace tdb.test.xUnit.Domain
{
    /// <summary>
    /// 验证值对象
    /// </summary>
    public class TestValueObject
    {
        /// <summary>
        /// 测试比较是否相等
        /// </summary>
        [Fact]
        public void TestEquals()
        {
            var val1 = new ValueObject1();
            var val11 = new ValueObject1();
            Assert.True(val1.Equals(val11));
            val11.Age += 1;
            Assert.False(val1.Equals(val11));

            var val2 = new ValueObject2();
            var val22 = new ValueObject2();
            Assert.True(val2.Equals(val22));
            val22.Age += 1;
            Assert.False(val2.Equals(val22));

            Assert.False(val1.Equals(2));
        }

        #region 定义

        /// <summary>
        /// 
        /// </summary>
        public class ValueObject1 : TdbValueObject
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime BirthDate;

            /// <summary>
            /// 
            /// </summary>
            public ValueObject1()
            {
                this.Name = "张三";
                this.Age = 13;
                this.BirthDate = DateTime.Today;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class ValueObject2 : TdbValueObject
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime BirthDate;
            
            /// <summary>
            /// 
            /// </summary>
            public ValueObject2()
            {
                this.Name = "张三";
                this.Age = 13;
                this.BirthDate = DateTime.Today;
            }
        }

        #endregion
    }
}
