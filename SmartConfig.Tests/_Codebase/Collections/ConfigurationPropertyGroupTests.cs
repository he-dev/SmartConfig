using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.Tests.Collections.ConfigurationPropertyGroupTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresConfigType()
        {
            new ConfigurationPropertyGroup(null);
        }
    }

    [TestClass]
    public class DataSource
    {
        [TestMethod]
        public void GetsDefaultOrCustom()
        {
            var propertyGroup = new ConfigurationPropertyGroup(typeof(TestConfigWithCustomDataSource));
            Assert.IsNotNull(propertyGroup.DataSource);
            Assert.IsInstanceOfType(propertyGroup.DataSource, typeof(SimpleTestDataSource));
        }

        [TestMethod]
        public void GetsDefaultIfNoPropertiesDefined()
        {
            var propertyGroup = new ConfigurationPropertyGroup(typeof(TestConfigWithoutProperties));
            Assert.IsNotNull(propertyGroup.DataSource);
            Assert.IsInstanceOfType(propertyGroup.DataSource, typeof(AppConfigSource));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsNullReferenceExceptionIfNull()
        {
            var propertyGroup = new ConfigurationPropertyGroup(typeof(TestConfigWithNullDataSource));
            var dataSource = propertyGroup.DataSource;
        }

        [SmartConfig]
        public static class TestConfigWithCustomDataSource
        {
            [SmartConfigProperties]
            public static class CustomProperties
            {
                public static IDataSource MyDataSource { get; } = new SimpleTestDataSource();
            }
        }

        [SmartConfig]
        public static class TestConfigWithNullDataSource
        {
            [SmartConfigProperties]
            public static class CustomProperties
            {
                public static IDataSource MyDataSource { get; } = null;
            }
        }

        [SmartConfig]
        public static class TestConfigWithoutProperties
        {
        }
    }

    [TestClass]
    public class CustomKeys
    {
        [TestMethod]
        public void GetsDefaultOrCustom()
        {
            var propertyGroup = new ConfigurationPropertyGroup(typeof(TestConfigWithCustomKeys));
            Assert.IsNotNull(propertyGroup.CustomKeys);
            Assert.AreEqual(1, propertyGroup.CustomKeys.Count());
        }

        [TestMethod]
        public void GetsDefaultIfNoPropertiesDefined()
        {
            var propertyGroup = new ConfigurationPropertyGroup(typeof(TestConfigWithoutProperties));
            Assert.IsNotNull(propertyGroup.CustomKeys);
            Assert.AreEqual(0, propertyGroup.CustomKeys.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsNullReferenceExceptionIfNull()
        {
            var propertyGroup = new ConfigurationPropertyGroup(typeof(TestConfigWithNullCustomKeys));
            var dataSource = propertyGroup.CustomKeys;
        }

        [SmartConfig]
        public static class TestConfigWithCustomKeys
        {
            [SmartConfigProperties]
            public static class CustomProperties
            {
                public static IDataSource MyDataSource { get; } = new SimpleTestDataSource();

                public static IEnumerable<SettingKey> MySettingKeys { get; } = new[]
                {
                        new SettingKey("Env", "foot"),
                    };
            }
        }

        [SmartConfig]
        public static class TestConfigWithNullCustomKeys
        {
            [SmartConfigProperties]
            public static class CustomProperties
            {
                public static IEnumerable<SettingKey> MySettingKeys { get; } = null;
            }
        }

        [SmartConfig]
        public static class TestConfigWithoutProperties
        {
        }
    }
}
