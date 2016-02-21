using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Filters;
using SmartUtilities.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Attributes.KeyFilterAttributeTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void RequiresFilterType()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new SettingFilterAttribute(null);
            }, null, Assert.Fail);
        }

        [TestMethod]
        public void RequiresFilterTypeIsIKeyFilter()
        {
            ExceptionAssert.Throws<TypeDoesNotImplementInterfaceException>(() =>
            {
                new SettingFilterAttribute(typeof(string));
            }, ex =>
            {
                Assert.AreEqual(typeof(ISettingFilter).FullName, ex.ExpectedType);
                Assert.AreEqual(typeof(string).FullName, ex.ActualType);
            }, Assert.Fail);
        }

        [TestMethod]
        public void CreatesKeyFilterAttribute()
        {
            new SettingFilterAttribute(typeof(StringFilter));
            Assert.IsTrue(true);
        }
    }
}
