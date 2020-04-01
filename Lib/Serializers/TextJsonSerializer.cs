using System.Text.Json;

namespace XJK.Serializers
{
    public class TextJsonSerializer : IStringSerializer
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        public string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize<T>(value, options);
        }

        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}
