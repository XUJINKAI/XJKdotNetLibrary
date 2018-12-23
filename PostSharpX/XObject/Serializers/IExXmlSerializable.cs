using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.XObject.Serializers
{
    public interface IExXmlSerializable
    {
        string ParseError { get; }
        string GetXmlData();
        void SetByXml(string xml);
    }
}
