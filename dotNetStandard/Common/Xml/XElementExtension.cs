﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace XJK.Xml
{
    public static class XElementExtension
    {
        public static string InnerXml(this XElement element)
        {
            var reader = element.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }

        public static string OuterXml(this XElement element)
        {
            var reader = element.CreateReader();
            reader.MoveToContent();
            return reader.ReadOuterXml();
        }
    }
}
