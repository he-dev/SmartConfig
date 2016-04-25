using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.DataAnnotations;
using SmartConfig.Filters;
using SmartUtilities.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.KeyFilterAttributeTests
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
            ExceptionAssert.Throws<ArgumentException>(() =>
            {
                new SettingFilterAttribute(typeof(string));
            }, ex =>
            {
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
