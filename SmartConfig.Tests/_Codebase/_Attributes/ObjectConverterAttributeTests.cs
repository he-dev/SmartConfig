using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartConfig.Filters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests.ObjectConverterAttributeTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void RequiresConverterType()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new ObjectConverterAttribute(null);
            }, null, Assert.Fail);
        }

        [TestMethod]
        public void RequiresConverterTypeIsObjectConverter()
        {
            ExceptionAssert.Throws<TypeDoesNotImplementInterfaceException>(() =>
            {
                new ObjectConverterAttribute(typeof(string));
            }, ex =>
            {
                Assert.AreEqual(typeof(ObjectConverter).FullName, ex.ExpectedType);
                Assert.AreEqual(typeof(string).FullName, ex.ActualType);
            }, Assert.Fail);
        }

        [TestMethod]
        public void CreatesObjectConverterAttribute()
        {
            new ObjectConverterAttribute(typeof(StringConverter));
            Assert.IsTrue(true);
        }
    }
}
