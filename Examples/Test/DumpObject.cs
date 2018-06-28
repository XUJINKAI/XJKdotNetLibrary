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
            int local1 = 111;
            string local2 = "xxx";
            try
            {
                ThrowExceptionInner("abc");
            }
            catch (Exception ex)
            {
                var thisType = typeof(DumpObject);
                var thisObject = typeof(DumpObject);
                var trace = new System.Diagnostics.StackTrace();
                var frame = trace.GetFrame(1);
                var methodName = frame.GetMethod().Name;
                var localVariables = frame.GetMethod().GetMethodBody().LocalVariables;
                Debug.WriteLine(methodName);

                foreach(var variable in localVariables)
                {
                    Debug.WriteLine($"{variable.LocalType.FullName}: {variable.ToString()}");
                }

                var properties = thisType.GetProperties();
                var fields = thisType.GetFields(); // public fields
                                                         // for example:
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(thisObject, null);
                    Debug.WriteLine($"{prop.Name}: {value}");
                }
                foreach (var field in fields)
                {
                    var value = field.GetValue(thisObject);
                    Debug.WriteLine($"{field.Name}: {value}");
                }
                
                ex.Data.Add("from", "ThrowExceptionInner(\"abc\");");
                ex.Data.Add("sender", typeof(DumpObject));
                throw new Exception("Outer Exception Msg", ex);
            }
        }

        public static void GetLocalVaribales()
        {
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
