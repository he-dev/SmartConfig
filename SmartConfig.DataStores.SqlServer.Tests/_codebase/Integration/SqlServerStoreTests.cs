using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace SmartConfig.DataStores.SqlServer.Tests.Integration.SqlServerStore.Positive
{
    using SqlServer;

    [TestClass]
    public class GetSettings
    {
        [TestMethod]
        public void ByName()
        {
            var config = Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Select(typeof(TestConfig));

            config.Settings.Count.Validate().IsEqual(1);
            TestConfig.Foo.Verify("TestConfig.Foo").IsEqual("Baar");
        }

        [TestMethod]
        public void ByNameAndNamespace()
        {
            var config = Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Where("Environment", "Corge")
                .Select(typeof(TestConfig2));

            config.Settings.Count.Validate().IsEqual(1);
            TestConfig2.Rox.Verify("TestConfig.Rox").IsNotNullOrEmpty().IsEqual("Quux");
        }

        [SmartConfig]
        private static class TestConfig
        {
            public static string Foo { get; set; }
        }

        [SmartConfig]
        private static class TestConfig2
        {
            public static string Rox { get; set; }
        }
    }

    [TestClass]
    public class SaveSetting
    {
        [TestMethod]
        public void SaveSettingByName()
        {
            Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Select(typeof(TestConfig1));

            var rnd = new Random();
            var newValue = $"{nameof(TestConfig1.Rox)}{rnd.Next(101)}";
            TestConfig1.Rox = newValue;

            Configuration.Save(typeof(TestConfig1));
            TestConfig1.Rox = string.Empty;
            Configuration.Reload(typeof(TestConfig1));
            TestConfig1.Rox.Verify().IsNotNullOrEmpty().IsEqual(newValue);
        }

        [TestMethod]
        public void SaveSettingByNameAndNamespace()
        {
            Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Where("Environment", "Qux")
                .Select(typeof(TestConfig2));

            var rnd = new Random();
            var newValue = $"{nameof(TestConfig2.Bar)}{rnd.Next(101)}";
            TestConfig2.Bar = newValue;

            Configuration.Save(typeof(TestConfig2));
            TestConfig2.Bar = string.Empty;
            Configuration.Reload(typeof(TestConfig2));
            TestConfig2.Bar.Verify().IsNotNullOrEmpty().IsEqual(newValue);
        }

        [SmartConfig]
        private static class TestConfig1
        {
            public static string Rox { get; set; }
        }

        [SmartConfig]
        private static class TestConfig2
        {
            [Optional]
            public static string Bar { get; set; }
        }
    }
}

namespace SmartConfig.DataStores.SqlServer.Tests.Integration.SqlServerStore.Negative
{
}