using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class ReflectionExtensionsTests
    {
        public static  class StaticTestClass { }

        public class NonStaticTestClass { }

        [TestMethod]
        public void IsStatic_True()
        {
            Assert.IsTrue(typeof (StaticTestClass).IsStatic());
        }

        [TestMethod]
        public void IsStatic_False()
        {
            Assert.IsFalse(typeof(NonStaticTestClass).IsStatic());
        }

        [TestMethod]
        public void GetTypes_GetsAllClasses()
        {
            var types = typeof (TestTypeRoot).GetTypes(null).ToList();
            Assert.AreEqual(6, types.Count);
        }
    }

    [SmartConfig]
    static class TestTypeRoot
    {
        public static class SubType1 { }

        public static class SubType2
        {
            public static class SubSubType1
            {
                public static class SubSubSubType1 { }
            }

            public static class SubSubType2 { }
        }
    }
}