using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

namespace SmartConfig.DataStores.AppConfig.Tests.Integration.AppSettingsStore.Positive
{
    using AppConfig;

    [TestClass]
    public class GetSettings
    {
        [TestMethod]
        public void GetSettingsSimple()
        {
            Configuration.Load.From(new AppSettingsStore()).Select(typeof(FullConfig1));

            FullConfig1.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foo");
            FullConfig1.ArraySetting.Length.Verify().IsEqual(2);
            FullConfig1.DictionarySetting.Count.Verify().IsEqual(2);
            FullConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            FullConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }

        [TestMethod]
        public void GetSettingsWithConfigNameAsPath()
        {
            Configuration.Load.From(new AppSettingsStore()).Select(typeof(FullConfig2));

            FullConfig2.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foox");
            FullConfig2.ArraySetting.Length.Verify().IsEqual(2);
            FullConfig2.DictionarySetting.Count.Verify().IsEqual(2);
            FullConfig2.NestedConfig.StringSetting.Verify().IsEqual("Barx");
            FullConfig2.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }
    }

    [TestClass]
    public class SaveSetting
    {
        [TestMethod]
        public void SaveSettingByName()
        {
            //var value = DateTime.UtcNow.ToFileTime().ToString();

            //Configuration.Load
            //    .From(new AppSettingsStore())
            //    .Select(typeof(TestConfig2));

            //TestConfig2.FileTime = value;
            //Configuration.Save(typeof(TestConfig2));
            //TestConfig2.FileTime = null;
            //Configuration.Reload(typeof(TestConfig2));
            //TestConfig2.FileTime.Verify().IsNotNullOrEmpty().IsEqual(value);
        }
    }
}

namespace SmartConfig.DataStores.AppConfig.Tests.Integration.AppSettingsStore.Positive.TestConfigs
{
    [SmartConfig]
    internal static class GetSettingsByName
    {
        public static string Foo { get; set; }
    }

    [SmartConfig]
    internal static class GetNestedSettingsByName
    {
        public static class Foo
        {
            public static string Bar { get; set; }
        }
    }

    [SmartConfig]
    internal static class TestConfig2
    {
        [Optional]
        public static string FileTime { get; set; }
    }
}

namespace SmartConfig.DataStores.AppConfig.Tests.Integration.AppSettingsStore.Negative
{

}
