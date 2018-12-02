using System;
using System.Collections.Generic;
using System.Text;
using XJK.Serializers;

namespace XJK.FileSystem
{
    public class ObjectXmlConverter<T> : IObjectFileConverter<T>
    {
        public void Convert(T obj, string FilePath)
        {
            FS.WriteAllText(FilePath, XmlSerialization.ToXmlText(obj));
        }

        public T ConvertBack(string FilePath)
        {
            return XmlSerialization.FromXmlText<T>(FS.ReadAllText(FilePath));
        }
    }
}
