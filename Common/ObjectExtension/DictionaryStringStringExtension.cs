using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XJKdotNetLibrary
{
    public static class DictionaryStringStringExtension
    {
        public static string ToFormatTableString(this Dictionary<string, string> dict)
        {
            string str;
            str = dict.Aggregate("", (sum, next) => $"{sum}{C.LF}{next.Key,-20} = {next.Value}");
            if (str.Length > 1) str = str.Substring(1);
            return str;
        }

        public static string ToUrlEncodeString(this Dictionary<string, string> dict, bool EncodePath = false)
        {
            string Trans(string s) => EncodePath ? HttpUtility.UrlPathEncode(s) : HttpUtility.UrlEncode(s);
            string str;
            str = dict.Aggregate("", (sum, next) => $"{sum}&{Trans(next.Key)}={Trans(next.Value)}");
            if (str.Length > 1) str = str.Substring(1);
            return str;
        }
    }
}
