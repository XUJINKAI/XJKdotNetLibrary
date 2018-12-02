using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK;
using XJK.Serializers;

namespace XJK.FileSystem
{
    public interface IObjectFileConverter<T>
    {
        void Convert(T obj, string filePath);
        T ConvertBack(string filePath);
    }
}
