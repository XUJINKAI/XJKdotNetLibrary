namespace XJK.Serializers
{
    public interface IStringSerializer
    {
        string Serialize<T>(T value);

        T Deserialize<T>(string text);
    }
}
