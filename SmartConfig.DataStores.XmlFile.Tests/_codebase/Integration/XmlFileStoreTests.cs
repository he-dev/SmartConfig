using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartConfig.DataStores.XmlFile;
using SmartConfig.DataStores.XmlFile.Tests;
using SmartUtilities.DataAnnotations;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

// ReSharper disable once CheckNamespace
namespace SmartConfig.DataStores.XmlFile.Tests.Integration.XmlFileStore.Positive
{
    using TestConfigs;

    [TestClass]
    public class TestBase
    {
        protected const string TestFileName = @"C:\Home\Projects\SmartConfig\SmartConfig.DataStores.XmlFile.Tests\bin\Debug\TestConfig.xml";
    }

    [TestClass]
    public class GetSettings : TestBase
    {
        [TestMethod]
        public void GetSettingsByName()
        {
            Configuration.Load
                .From(new XmlFile.XmlFileStore(TestFileName))
                .Select(typeof(TestConfig1));

            TestConfig1.Foo.Verify("TestConfig1.Foo").IsNotNullOrEmpty().IsEqual("Bar");
        }

        [TestMethod]
        public void GetSettingsByNameAndNamespace()
        {
            Configuration.Load
                .From(new XmlFile.XmlFileStore(TestFileName))
                .Where("Environment", "Baz")
                .Select(typeof(TestConfig2));
            TestConfig2.Bar.Verify("TestConfig2.Bar").IsNotNullOrEmpty().IsEqual("Qux");
        }
    }

    //[TestClass]
    //public class Update : TestBase
    //{
    //    [TestMethod]
    //    public void UpdatesCustomSetting()
    //    {
    //        Configuration
    //            .Load(typeof(Config1))
    //            .From(new XmlFileStore<CustomTestSetting>(TestFileName), dataStore =>
    //            {
    //                dataStore.SetCustomKey("Environment", "corge");
    //                dataStore.SetCustomKey("Version", "4.1.5");
    //            });

    //        Assert.AreEqual("plugh", Config1.Waldo);

    //        Config1.Waldo = "foox";
    //        Configuration.Save(typeof(Config1));


    //        Config1.Waldo = "fooxxx";
    //        Configuration.Reload(typeof(Config1));
    //        Assert.AreEqual("foox", Config1.Waldo);
    //    }

    //    [SmartConfig]
    //    private static class Config1
    //    {
    //        public static string Waldo { get; set; }
    //    }

    //    [TestMethod]
    //    public void AddsNewSetting()
    //    {
    //        Configuration
    //            .Load(typeof(Config2))
    //            .From(new XmlFileStore<BasicSetting>(TestFileName));

    //        Config2.Xyzzy = "thudy";
    //        Configuration.Save(typeof(Config2));

    //        Configuration.Reload(typeof(Config2));
    //        Assert.AreEqual("thudy", Config2.Xyzzy);
    //    }

    //    [SmartConfig]
    //    private static class Config2
    //    {
    //        [Optional]
    //        public static string Xyzzy { get; set; } = "thud";
    //    }

    //}
}

namespace SmartConfig.DataStores.XmlFile.Tests.Integration.XmlFileStore.Positive.TestConfigs
{
    [SmartConfig]
    internal static class TestConfig1
    {
        public static string Foo { get; set; }
    }

    [SmartConfig(Name = "ns")]
    internal static class TestConfig2
    {
        public static string Bar { get; set; }
    }

    [SmartConfig]
    internal static class TestConfig3
    {
        public static string Foo { get; set; }
    }
}