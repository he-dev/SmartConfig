using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests
{
    [TestClass()]
    public class TypeExtensionsTests
    {
        public static  class StaticTestClass { }

        public class NonStaticTestClass { }

        [TestMethod()]
        public void IsStatic_True()
        {
            Assert.IsTrue(typeof (StaticTestClass).IsStatic());
        }

        [TestMethod()]
        public void IsStatic_False()
        {
            Assert.IsFalse(typeof(NonStaticTestClass).IsStatic());
        }
    }
}