using System;
using System.Collections.Generic;
using System.Text;
using XJK.Serializers;
using Xunit;
using Xunit.Abstractions;

namespace XJK.Lib_Test.Serializers_Test
{
    public class SerializeDeserialize_Test : BaseTestClass
    {
        public SerializeDeserialize_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        // JsonNetSerializer can process complex struct such as interface or abstract class
        [InlineData(typeof(JsonNetSerializer), typeof(ComplexConfig))]
        // while TextJsonSerializer can only process simple struct
        [InlineData(typeof(TextJsonSerializer), typeof(SimpleConfig))]
        public void StringSerialization_Test(Type TSerializer, Type TConfig)
        {
            var serializer = Activator.CreateInstance(TSerializer) as IStringSerializer;
            if (serializer is null) throw new NullReferenceException();

            var config = Activator.CreateInstance(TConfig);

            var json = serializer.Serialize(config);
            WriteLine(json);

            var restore = serializer.Deserialize<ComplexConfig>(json);
        }

        [Theory]
        [InlineData(typeof(SharpBinarySerializer))]
        public void BytesSerialization_Test(Type TSerializer)
        {
            var serializer = Activator.CreateInstance(TSerializer) as IBytesSerializer;
            if (serializer is null) throw new NullReferenceException();

            var config = new ComplexConfig();

            var bytes = serializer.SerializeToBinary(config);
            WriteLine(Convert.ToBase64String(bytes));

            var restore = serializer.DeserializeFromBinary<ComplexConfig>(bytes);
        }
    }
}
