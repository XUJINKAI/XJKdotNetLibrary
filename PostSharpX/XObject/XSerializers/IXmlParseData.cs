using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.XSerializers
{
    public interface IXmlParseData
    {
        string ParseError { get; }
        string GetXmlData();
        void SetByXml(string xml);
    }
}
