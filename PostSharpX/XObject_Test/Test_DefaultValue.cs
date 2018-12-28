using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XJK.XObject_Test
{
    /// <summary>
    /// DefaultValueTest 的摘要说明
    /// </summary>
    [TestClass]
    public class Test_DefaultValue
    {
        Database database;

        public Test_DefaultValue()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        [TestInitialize()]
        public void MyTestInitialize()
        {
            database = new Database();
        }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Test_DefaultValueAttribute()
        {
            Assert.AreEqual(Database.DefaultValue_Int_Def, database.DefaultValue_Int);
            Assert.AreEqual(Database.DefaultValue_String_Def, database.DefaultValue_String);
        }

        [TestMethod]
        public void Test_DefaultValueByMethodAttribute()
        {
            Assert.AreEqual(database.DefaultValueByMethod_String_Def(), database.DefaultValueByMethod_String);
        }

        [TestMethod]
        public void Test_DefaultValueNewInstanceAttribute()
        {
            Assert.IsNotNull(database.DefaultValueNewInstance_Instance);
        }

        [TestMethod]
        public void Test_ResetAllProperties()
        {
            database.RandomSetProperties();
            TestContext.WriteLine("Before:");
            TestContext.WriteLine(database.GetXmlData());

            database.ResetAllPropertiesDefaultValue();
            TestContext.WriteLine("After:");
            TestContext.WriteLine(database.GetXmlData());

            Test_DefaultValueAttribute();
            Test_DefaultValueByMethodAttribute();
            Test_DefaultValueNewInstanceAttribute();
        }

        [TestMethod]
        public void Test_SubClass_Property()
        {
            Assert.IsNotNull(database.SubDatabase.Collection);
        }
    }
}
