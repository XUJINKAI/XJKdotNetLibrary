namespace XJK.FileSystem
{
    public class ObjectXmlConverter : IObjectFileConverter
    {
        public void Convert<T>(T obj, string FilePath)
        {
            FS.WriteAllText(FilePath, XmlSerialization.ToXmlText(obj));
        }

        public T ConvertBack<T>(string FilePath)
        {
            return XmlSerialization.FromXmlText<T>(FS.ReadAllText(FilePath));
        }
    }
}
