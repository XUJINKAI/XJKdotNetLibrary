namespace XJK.Serializers
{
    public interface IBytesSerializer
    {
        byte[] SerializeToBinary<T>(T value);

        T DeserializeFromBinary<T>(byte[] bytes);
    }
}
