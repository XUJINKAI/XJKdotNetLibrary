using Newtonsoft.Json;

namespace XJK.Serializers
{
    public class JsonNetSerializer : IStringSerializer
    {
        private readonly JsonSerializerSettings _setting;

        public JsonNetSerializer()
        {
            _setting = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
            };
        }

        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, typeof(T), _setting);
        }

        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, _setting);
        }
    }
}
