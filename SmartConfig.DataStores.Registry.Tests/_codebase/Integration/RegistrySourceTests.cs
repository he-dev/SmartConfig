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
        private const string TestRegistryKey = @"Software\SmartConfig\Tests";

        [TestMethod]
        public void GetSettingsSimple()
        {
            Configuration.Load.From(RegistryStore.CreateForCurrentUser(TestRegistryKey)).Select(typeof(FullConfig1));

            FullConfig1.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foo");
            FullConfig1.ArraySetting.Length.Verify().IsEqual(2);
            FullConfig1.DictionarySetting.Count.Verify().IsEqual(2);
            FullConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            FullConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }

        [TestMethod]
        public void GetSettingsWithConfigNameAsPath()
        {
            Configuration.Load.From(RegistryStore.CreateForCurrentUser(TestRegistryKey)).Select(typeof(FullConfig2));

            FullConfig2.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foox");
            FullConfig2.ArraySetting.Length.Verify().IsEqual(2);
            FullConfig2.DictionarySetting.Count.Verify().IsEqual(2);
            FullConfig2.NestedConfig.StringSetting.Verify().IsEqual("Barx");
            FullConfig2.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
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
