using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using XJK.DefaultProperty;

namespace XJK.DefaultProperty_Test
{
    class TestClass : IDefaultPropertyExtension
    {
        public const string DefNameValue = "XJK";

        [DefaultValue(DefNameValue)] public string Name { get; set; }

        [DefaultValueByMethod(nameof(ReturnInt1))] public int Age { get; set; }

        [DefaultValueNewInstance] public object Object { get; set; }

        public int ReturnInt1()
        {
            return 1;
        }
    }
}
