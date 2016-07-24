using System;
using System.Linq;
using System.Web.ApplicationServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;
using SmartUtilities.TypeFramework;
using SmartUtilities.TypeFramework.Converters;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

namespace SmartConfig.DataStores.Registry.Tests.Integration.RegistryStore.Positive
{
    using Registry;

    [TestClass]
    public class GetSettings
    {
        [TestMethod]
        public void GetSettingsByName()
        {
            Configuration.Load
                .From(RegistryStore.CreateForCurrentUser(@"Software\SmartConfig\Tests"))
                .Select(typeof(TestConfig));

            TestConfig.Foo.Verify().IsEqual("baz");
            TestConfig.Bar.Verify().IsEqual(123);
            TestConfig.Quux.Verify().IsEqual(TestEnum.TestValue2);
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
    public class SaveSettings
    {
        [TestMethod]
        public void SaveSettingsByName()
        {
            Configuration.Load
                .From(RegistryStore.CreateForCurrentUser(@"Software\SmartConfig\Tests"))
                .Select(typeof(TestConfig));

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
