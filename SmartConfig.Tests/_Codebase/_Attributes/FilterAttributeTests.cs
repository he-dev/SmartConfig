using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Filters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class FilterAttributeTests_ctor
    {
        [TestMethod]
        public void RequiresFilterType()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new FilterAttribute(null);
            }, null, Assert.Fail);
        }

        [TestMethod]
        public void RequiresFilterTypeImplementsISettingFilter()
        {
            ExceptionAssert.Throws<TypeDoesNotImplementInterfaceException>(() =>
            {
                new FilterAttribute(typeof(string));
            }, ex =>
            {
                Assert.AreEqual(typeof(ISettingFilter).FullName, ex.InterfaceTypeName);
                Assert.AreEqual(typeof(string).FullName, ex.InvalidTypeName);
            }, Assert.Fail);
        }
    }
}
