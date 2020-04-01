using System;

namespace XJK.Lib_Test.Serializers_Test
{
    public interface ISimpleConfig
    {
        string SubName { get; set; }
    }

    public interface IComplexConfig
    {
        string MainName { get; set; }
        ISimpleConfig SubA { get; set; }
    }

    // Impl

    public class SimpleConfig : ISimpleConfig
    {
        public string SubName { get; set; }
        public int ValueA { get; set; }
    }

    public class ComplexConfig : IComplexConfig
    {
        public string MainName { get; set; }
        public ISimpleConfig SubA { get; set; }
        public ISimpleConfig SubB { get; set; }

        public ComplexConfig()
        {
            MainName = "MainConfig_Name";
            SubA = new SimpleConfig()
            {
                SubName = "SubConfig_NameA",
                ValueA = 123,
            };
        }
    }
}
