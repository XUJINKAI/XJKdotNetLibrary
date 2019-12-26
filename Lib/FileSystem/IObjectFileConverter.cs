namespace XJK.FileSystem
{
    public interface IObjectFileConverter
    {
        void Convert<T>(T obj, string filePath);
        T ConvertBack<T>(string filePath);
    }
}
