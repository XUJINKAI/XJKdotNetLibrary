using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XJK.AOP
{
    [Serializable]
    public class MethodCallInfo
    {
        public string Name { get; set; }
        public List<Object> Args { get; set; }
        public object Result { get; set; } = null;
        public string Exception { get; set; } = null;

        public MethodCallInfo()
        {
            Args = new List<object>();
        }

        public object Excute(object obj)
        {
            Type[] types = Args.Select(o => o.GetType()).ToArray();
            var method = obj.GetType().GetMethod(Name, types);
            var invokeresult = method.Invoke(obj, Args.ToArray());
            if(invokeresult is Task)
            {
                Task.Run(async () => { await (Task)invokeresult; });
                return invokeresult.GetType().GetMethod("get_Result").Invoke(invokeresult, null);
            }
            return invokeresult;
        }

        public override string ToString()
        {
            string s = Name;
            if (Args.Count != 0)
            {
                s += " {" + string.Join(",", Args) + "}";
            }
            if (Result != null)
            {
                s += ": " + Result.ToString();
            }
            return s;
        }
    }
}
