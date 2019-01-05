using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XJK.XAspects;

namespace XAspects_Test
{
    [TestClass]
    public class Test_CanOnlyRunOnce
    {
        [TestMethod]
        public void Test_MethodGood()
        {
            var x = new Instance();
            x.MethodInstanceScope();
        }

        [TestMethod]
        public void Test_MethodFail()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var x = new Instance();
                x.MethodInstanceScope();
                x.MethodInstanceScope();
            });
        }

        [TestMethod]
        public void Test_MultiInstance_MethodGood()
        {
            var x = new Instance();
            x.MethodInstanceScope();
            var y = new Instance();
            y.MethodInstanceScope();
        }

        [TestMethod]
        public void Test_MultiInstance_MethodFail()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var x = new Instance();
                x.MethodStaticScope();
                var y = new Instance();
                y.MethodStaticScope();
            });
        }
    }
    
    class Instance
    {
        public Instance() { }

        [CanOnlyRunOnce]
        public void MethodInstanceScope() { }

        [CanOnlyRunOnce(StaticScoped = true)]
        public void MethodStaticScope() { }
    }
}
