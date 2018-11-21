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
        private readonly object _defValue;

        public DatabasePropertyAttribute(object defValue)
        {
            _defValue = defValue;
        }

        public override bool CompileTimeValidate(LocationInfo locationInfo)
        {
            if (_defValue == null && locationInfo.LocationType.IsValueType
                || _defValue != null && locationInfo.LocationType != _defValue.GetType())
                throw new TypeInitializationException(locationInfo.Name, null);
            return base.CompileTimeValidate(locationInfo);
        }

        public override void OnGetValue(LocationInterceptionArgs args)
        {
            Database db = (Database)args.Instance;
            args.Value = db.Get(args.LocationName, _defValue);
        }

        public override void OnSetValue(LocationInterceptionArgs args)
        {
            Database db = (Database)args.Instance;
            db[args.LocationName] = args.Value;
        }
    }
}
