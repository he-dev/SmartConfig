using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.Tests.Collections
{
    [TestClass]
    public class ConfigurationPropertyGroupTests_ctor
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresConfigType()
        {
            new ConfigurationPropertyGroup(null);
        }

        [TestMethod]
        public void InitializesSmartConfigProperties()
        {
            var propertyGroup = new ConfigurationPropertyGroup(typeof (TestConfigWithInterfaces));
            Assert.IsNotNull(propertyGroup.DataSource);
            Assert.IsNotNull(propertyGroup.CustomKeys);
            Assert.AreEqual(1, propertyGroup.CustomKeys.Count());
        }

        [SmartConfig]
        public static class TestConfigWithInterfaces
        {
            [SmartConfigProperties]
            public static class CustomProperties
            {
                public static IDataSource AppConfigDataSource { get; } = new AppConfigSource();

                public static IEnumerable<SettingKey> MySettingKeys { get; } = new[]
                {
                    new SettingKey("Env", "foot"),
                };
            }
        }

        [SmartConfig]
        public static class TestConfigWithClasses
        {
            [SmartConfigProperties]
            public static class CustomProperties
            {
                public static IDataSource AppConfigDataSource { get; } = new AppConfigSource();

                public static SettingKey[] MySettingKeys { get; } = new[]
                {
                    new SettingKey("Env", "foot"),
                };
            }
        }
    }
}
