using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.ObjectConverters.DataAnnotations;
using SmartUtilities.UnitTesting;

namespace SmartConfig.DataStores.Registry.Tests.RegistryStoreTests
{
    [TestClass]
    public class ConstructorTests
    {
        [TestMethod]
        public void RequiresBaseRegistryKey()
        {
            ExceptionAssert.Throws<ArgumentNullException>(delegate
            {
                var registryStore = new RegistryStore(null, null);
            }, null, Assert.Fail);
        }

        [TestMethod]
        public void RequiresRegistrySubKey()
        {
            ExceptionAssert.Throws<ArgumentNullException>(delegate 
            {
                var registryStore = new RegistryStore(Microsoft.Win32.Registry.CurrentUser, null);
            }, null, Assert.Fail);
        }
    }

    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void SelectsSettingsWithoutModelName()
        {
            Configuration
                .Load(typeof(TestConfig))
                .From(new RegistryStore(Microsoft.Win32.Registry.CurrentUser, @"Software\SmartConfig\Tests"));

            Assert.AreEqual("baz", TestConfig.Foo);
            Assert.AreEqual(123, TestConfig.Bar);
            Assert.AreEqual(TestEnum.TestValue2, TestConfig.Quux);
        }

        [SmartConfig]
        private static class TestConfig
        {
            public static string Foo { get; set; }
            public static int Bar { get; set; }
            public static TestEnum Quux { get; set; }
        }

        public enum TestEnum
        {
            TestValue1,
            TestValue2,
            TestValue3
        }
    }

    [TestClass]
    public class UpdateTests
    {
        [TestMethod]
        public void UpdatesStringSettingByName()
        {
            Configuration
                .Load(typeof(TestConfig))
                .From(new RegistryStore(Microsoft.Win32.Registry.CurrentUser, @"Software\SmartConfig\Tests"));

            var now = DateTime.Now;
            TestConfig.Qux = now;
            Configuration.Save(typeof(TestConfig));

            // change the last value that should be overwritten on reload
            TestConfig.Qux = DateTime.MinValue;
            Configuration.Reload(typeof(TestConfig));
            Assert.AreEqual(now.ToLongDateString(), TestConfig.Qux.ToLongDateString());
        }

        [SmartConfig]
        private static class TestConfig
        {
            [Optional]
            public static DateTime Qux { get; set; }
        }
    }
}
