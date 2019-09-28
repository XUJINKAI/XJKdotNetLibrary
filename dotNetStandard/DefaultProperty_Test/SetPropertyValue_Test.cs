using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using XJK.DefaultProperty;

namespace XJK.DefaultProperty_Test
{
    public class SetPropertyValue_Test
    {
        [Fact]
        public void ValueType()
        {
            var cls = new TestClass();

            cls.ResetAllProperties();

            Assert.Equal(TestClass.DefNameValue, cls.Name);
            Assert.True(cls.Age == 1);
            Assert.True(cls.Object != null);
        }
    }
}
