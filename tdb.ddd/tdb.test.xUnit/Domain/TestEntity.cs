using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.domain;

namespace tdb.test.xUnit.Domain
{
    /// <summary>
    /// 验证实体
    /// </summary>
    public class TestEntity
    {
        /// <summary>
        /// 测试比较是否相等
        /// </summary>
        [Fact]
        public void TestEquals()
        {
            var val1 = new Entity1();
            var val11 = new Entity1();
            Assert.True(val1.Equals(val11));
            val11.Age += 1;
            Assert.False(val1.Equals(val11));
            Assert.False(val11.BigValue.IsLoaded);
            val11.BigValue.SetValue("abc");
            Assert.True(val11.BigValue.IsLoaded);
            Assert.Equal("abc", val11.BigValue.Value);

            var val2 = new Entity2();
            var val22 = new Entity2();
            Assert.True(val2.Equals(val22));
            val22.Age += 1;
            Assert.False(val2.Equals(val22));
            Assert.False(val22.BigValue.IsLoaded);
            var bigVal = val22.BigValue.Value;
            Assert.True(val22.BigValue.IsLoaded);
            Assert.Equal("big data", val22.BigValue.Value);
            Assert.Equal("big data", bigVal);

            Assert.False(val1.Equals(2));
        }

        #region 定义

        /// <summary>
        /// 
        /// </summary>
        public class Entity1 : TdbEntity<long>
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime BirthDate;
            /// <summary>
            /// 大值（懒加载）
            /// </summary>
            public ValueLazyLoad BigValue { get; set; } = new ValueLazyLoad();

            /// <summary>
            /// 
            /// </summary>
            public Entity1()
            {
                this.ID = 1;
                this.Name = "张三";
                this.Age = 13;
                this.BirthDate = DateTime.Today;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Entity2 : TdbEntity<long>
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime BirthDate;
            /// <summary>
            /// 大值（懒加载）
            /// </summary>
            public ValueLazyLoad BigValue { get; set; } = new ValueLazyLoad();

            /// <summary>
            /// 
            /// </summary>
            public Entity2()
            {
                this.ID = 1;
                this.Name = "张三";
                this.Age = 13;
                this.BirthDate = DateTime.Today;
            }
        }

        /// <summary>
        /// 懒加载
        /// </summary>
        public class ValueLazyLoad : TdbLazyLoadObject<string>
        {
            /// <summary>
            /// 加载
            /// </summary>
            /// <returns></returns>
            protected override string Load()
            {
                return "big data";
            }

            /// <summary>
            /// 赋值
            /// </summary>
            /// <param name="value">值</param>
            public void SetValue(string value)
            {
                this.Value = value;
            }
        }

        #endregion
    }
}
