using PostSharp.Aspects;
using PostSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XJK.Objects
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class DatabasePropertyAttribute : LocationInterceptionAspect
    {
        public bool AutoSetKey { get; set; }
        
        public override void OnGetValue(LocationInterceptionArgs args)
        {
            Database db = (Database)args.Instance;
            string key = args.LocationName;

            if (db.HasKey(key))
            {
                args.Value = db[key];
            }
            else
            {
                args.ProceedGetValue();
            }

            if (AutoSetKey && !db.HasKey(key))
            {
                db[key] = args.Value;
            }
        }

        public override void OnSetValue(LocationInterceptionArgs args)
        {
            args.ProceedSetValue();
            Database db = (Database)args.Instance;
            db[args.LocationName] = args.Value;
        }
    }
}
