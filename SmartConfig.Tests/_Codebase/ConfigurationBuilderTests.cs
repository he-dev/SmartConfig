using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.ConfigurationBuilderTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void RequiresConfigType()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new ConfigurationBuilder(null);
            }, ex => { Assert.AreEqual("configurationType", ex.ParamName); },
            Assert.Fail);
        }

        [TestMethod]
        public void RequiresSmartConfigAttribute()
        {
            ExceptionAssert.Throws<SmartConfigAttributeMissingException>(() =>
            {
                new ConfigurationBuilder(typeof(Foo));
            }, ex =>
            {
                Assert.AreEqual(typeof(Foo).FullName, ex.ConfigurationType);
            }, 
            Assert.Fail);
        }      

        [TestMethod]
        public void RequiresTypeIsStatic()
        {
            ExceptionAssert.Throws<TypeNotStaticException>(() =>
            {
                new ConfigurationBuilder(typeof(Bar));
            }, ex =>
            {
                Assert.AreEqual(typeof(Bar).FullName, ex.Type);
            },
            Assert.Fail);
        }

        [TestMethod]
        public void CreatesConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder(typeof(Baz));
            Assert.IsNotNull(configurationBuilder.Configuration);
        }

        static class Foo { }

        class Bar { }

        [SmartConfig]
        static class Baz { }
    }
}