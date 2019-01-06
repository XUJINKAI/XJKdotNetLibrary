using PostSharp.Patterns.Collections;
using PostSharp.Patterns.Model;
using PostSharp.Patterns.Recording;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK.XObject;
using XJK.XObject.DefaultProperty;
using static XJK.RandomGenerator;

namespace XJK.XObject_Test
{
    [Aggregatable]
    [Recordable]
    public class Database : DatabaseObject
    {

        public const int DefaultValue_Int_Def = 42;
        public const string DefaultValue_String_Def = "XJK.XObject_Test.DefaultValue_String_Def";
        public string DefaultValueByMethod_String_Def() { return "XJK.XObject_Test.DefaultValueByMethod_String_Def()"; }

        [DefaultValue(DefaultValue_Int_Def)] public int DefaultValue_Int { get; set; }
        [DefaultValue(DefaultValue_String_Def)] public string DefaultValue_String { get; set; }
        [DefaultValueByMethod(nameof(DefaultValueByMethod_String_Def))] public string DefaultValueByMethod_String { get; set; }
        [Child] [DefaultValueNewInstance] public SubInstance DefaultValueNewInstance_Instance { get; set; }
        [Child] public SubInstanceBase SubInstanceBase { get; set; }
        [Child] public NoCtorInstance NoCtorInstance { get; }

        [Child] public SubDatabase SubDatabase { get; }

        public Database()
        {
            SubInstanceBase = new SubInstance();
            SubDatabase = new SubDatabase();
            NoCtorInstance = new NoCtorInstance(1);
        }

        public IAggregatable AsIAggregatable() { return PostSharp.Post.Cast<Database, IAggregatable>(this); }

        public void RandomSetProperties()
        {
            DefaultValue_Int = RandomInt(100);
            DefaultValue_String = RandomString(10);
            DefaultValueByMethod_String = RandomString(10);
            DefaultValueNewInstance_Instance = new SubInstance()
            {
                Field = RandomGuid()
            };
            int i = RandomInt(5, 10);
            while (i-- > 0)
            {
                SubDatabase.Collection.Add(new SubInstance() { Field = RandomGuid() });
            }
        }
    }

    [Aggregatable]
    [Recordable]
    public class SubDatabase : DatabaseObject
    {
        [Child] public DataCollection<SubInstance> Collection { get; }
        [Child] public DataDictionary<string, SubInstance> Dictionary { get; }
        /// <summary>
        /// Cause string is not recordable.
        /// </summary>
        [Child] public AdvisableCollection<string> NotRecordableCollection { get; }

        public SubDatabase()
        {
            Collection = new DataCollection<SubInstance>();
            Dictionary = new DataDictionary<string, SubInstance>();
            NotRecordableCollection = new AdvisableCollection<string>();
        }
    }

    [Aggregatable]
    [Recordable]
    public class SubInstance : SubInstanceBase
    {
        [DefaultValue("")] public string Field { get; set; }
    }

    [Aggregatable]
    [Recordable]
    public class SubInstanceBase : NotifyObject
    {
        [DefaultValue("")] public string BaseField { get; set; }
    }
    
    [Aggregatable]
    [Recordable]
    public class NoCtorInstance : DatabaseObject
    {
        public string Field { get; set; }

        public NoCtorInstance(int i)
        {
            Field = RandomGuid();
        }
    }
}
