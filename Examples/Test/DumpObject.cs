using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK;

namespace Test
{
    static class DumpObject
    {
        public static string DumpObjectStaticMember = "DumpObjectStaticMember";
        public static string DumpObjectStaticProperty { get; set; } = "DumpObjectStaticProperty";

        public static void ThrowExceptionInner(string arg1, bool arg2 = false)
        {
            int local_variable = 123;
            throw new Exception("this is an Exception Message");
        }

        public static void ThrowException()
        {
            try
            {
                ThrowExceptionInner("abc");
            }
            catch (Exception ex)
            {
                ex.Data.Add("from", "ThrowExceptionInner(\"abc\");");
                ex.Data.Add("sender", typeof(DumpObject));
                throw new Exception("Outer Exception Msg", ex);
            }
        }

        public static void Test()
        {
            try
            {
                ThrowException();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                Trace.WriteLine(C.LF + typeof(Environment).Dump());
                Trace.WriteLine(C.LF + ex.Dump());
            }
        }
    }
}
