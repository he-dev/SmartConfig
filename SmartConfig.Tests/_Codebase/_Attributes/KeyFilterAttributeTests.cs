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
                new KeyFilterAttribute(null);
            }, null, Assert.Fail);
        }

        [TestMethod]
        public void RequiresFilterTypeIsIKeyFilter()
        {
            ExceptionAssert.Throws<TypeDoesNotImplementInterfaceException>(() =>
            {
                new KeyFilterAttribute(typeof(string));
            }, ex =>
            {
                Assert.AreEqual(typeof(IKeyFilter).FullName, ex.ExpectedType);
                Assert.AreEqual(typeof(string).FullName, ex.ActualType);
            }, Assert.Fail);
        }

        [TestMethod]
        public void CreatesKeyFilterAttribute()
        {
            new KeyFilterAttribute(typeof(StringKeyFilter));
            Assert.IsTrue(true);
        }
    }
}
