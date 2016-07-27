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

namespace SmartConfig.DataStores.AppConfig.Tests.Integration.ConnectionStringsStore.Positive
{
    using AppConfig;
    using TestConfigs;

    [TestClass]
    public class GetSettings
    {
        [TestMethod]
        public void GetSettingsSimple()
        {
            Configuration.Load.From(new ConnectionStringsStore()).Select(typeof(FullConfig3));

            FullConfig3.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foox");
            FullConfig3.NestedConfig.StringSetting.Verify().IsEqual("Barx");
            FullConfig3.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }

        [TestMethod]
        public void GetSettingsWithConfigNameAsPath()
        {
            Configuration.Load.From(new ConnectionStringsStore()).Select(typeof(FullConfig4));

            FullConfig4.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Fooxy");
            FullConfig4.NestedConfig.StringSetting.Verify().IsEqual("Barxy");
            FullConfig4.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }
    }
    [TestClass]
    public class SaveSetting
    {
        [TestMethod]
        public void SaveSettingByKey()
        {
            var value = DateTime.UtcNow.ToFileTime().ToString();

            Configuration.Load
                .From(new ConnectionStringsStore())
                .Select(typeof(TestConfig2));

            TestConfig2.FileTime = value;
            Configuration.Save(typeof(TestConfig2));
            TestConfig2.FileTime = null;
            Configuration.Reload(typeof(TestConfig2));
            TestConfig2.FileTime.Verify().IsNotNullOrEmpty().IsEqual(value);
        }
    }
}

namespace SmartConfig.DataStores.AppConfig.Tests.Integration.ConnectionStringsStore.Positive.TestConfigs
{
    [SmartConfig("Bar")]
    internal static class TestConfig1
    {
        public static string Foo { get; set; }
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
