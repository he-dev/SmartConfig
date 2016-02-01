using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartConfig.Reflection;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Reflection.ConfigurationInfoTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequriesConfigurationType()
        {
            Configuration.LoadSettings(null);
        }

        [TestMethod]
        [ExpectedException(typeof(SmartConfigAttributeMissingException))]
        public void RequiersConfigurationTypeHasSmartConfigAttribute()
        {
            Configuration.LoadSettings(typeof(Foo));
        }

        [TestMethod]
        [ExpectedException(typeof(TypeNotStaticException))]
        public void RequiresConfigurationTypeIsStatic()
        {
            Configuration.LoadSettings(typeof(Bar));
        }

        [TestMethod]
        public void InitialzesConfigurationPropertyGroup()
        {
            var configurationInfo = new ConfigurationInfo(typeof(Baz));
            Assert.IsNotNull(configurationInfo.ConfigurationPropertyGroup);
        }

        static class Foo { }

        class Bar { }

        [SmartConfig]
        static class Baz { }
    }

    [TestClass]
    public class ConfigurationName
    {
        [TestMethod]
        public void GetsNullIfNotSpecified()
        {
            Assert.IsNull(new ConfigurationInfo(typeof(Baz)).ConfigurationName);
        }

        [TestMethod]
        public void GetsStringIfSpecified()
        {
            Assert.AreEqual(
                "Bar",
                new ConfigurationInfo(typeof(Foo)).ConfigurationName);
        }

        [SmartConfig]
        static class Baz { }

        [SmartConfig]
        [SettingName("Bar")]
        static class Foo { }
    }
}