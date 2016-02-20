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
            new ConfigurationProperties(null);
        }
    }

    [TestClass]
    public class DataSource
    {
        [TestMethod]
        public void GetsDefaultOrCustom()
        {
            var propertyGroup = new ConfigurationProperties(typeof(TestConfigWithCustomDataSource));
            Assert.IsNotNull(propertyGroup.DataSource);
            Assert.IsInstanceOfType(propertyGroup.DataSource, typeof(SimpleTestDataStore));
        }

        [TestMethod]
        public void GetsDefaultIfNoPropertiesDefined()
        {
            var propertyGroup = new ConfigurationProperties(typeof(TestConfigWithoutProperties));
            Assert.IsNotNull(propertyGroup.DataSource);
            //Assert.IsInstanceOfType(propertyGroup.DataSource, typeof(AppConfigSource));
            Assert.Inconclusive("need data source?");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsNullReferenceExceptionIfNull()
        {
            var propertyGroup = new ConfigurationProperties(typeof(TestConfigWithNullDataSource));
            var dataSource = propertyGroup.DataSource;
        }

        [SmartConfig]
        public static class TestConfigWithCustomDataSource
        {
            [SmartConfigProperties]
            public static class CustomProperties
            {
                public static IDataStore MyDataSource { get; } = new SimpleTestDataStore();
            }
        }

        [SmartConfig]
        public static class TestConfigWithNullDataSource
        {
            [SmartConfigProperties]
            public static class CustomProperties
            {
                public static IDataStore MyDataSource { get; } = null;
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
            var propertyGroup = new ConfigurationProperties(typeof(TestConfigWithCustomKeys));
            Assert.IsNotNull(propertyGroup.CustomKeys);
            Assert.AreEqual(1, propertyGroup.CustomKeys.Count());
        }

        [TestMethod]
        public void GetsDefaultIfNoPropertiesDefined()
        {
            var propertyGroup = new ConfigurationProperties(typeof(TestConfigWithoutProperties));
            Assert.IsNotNull(propertyGroup.CustomKeys);
            Assert.AreEqual(0, propertyGroup.CustomKeys.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ThrowsNullReferenceExceptionIfNull()
        {
            var propertyGroup = new ConfigurationProperties(typeof(TestConfigWithNullCustomKeys));
            var dataSource = propertyGroup.CustomKeys;
        }

        [SmartConfig]
        public static class TestConfigWithCustomKeys
        {
            [SmartConfigProperties]
            public static class CustomProperties
            {
                public static IDataStore MyDataSource { get; } = new SimpleTestDataStore();

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
