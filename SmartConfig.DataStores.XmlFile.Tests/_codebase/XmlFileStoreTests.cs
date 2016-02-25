using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.DataStores.XmlFile;
using SmartConfig.DataStores.XmlFile.Tests;

// ReSharper disable once CheckNamespace
namespace SmartConfig.DataStores.XmlFile.Tests.XmlFileStoreTests
{
    //[TestClass]
    public class XmlFileStoreTestsBase
    {
        protected const string TestFileName = @"C:\Home\Projects\SmartConfig\SmartConfig.DataStores.XmlFile.Tests\bin\Debug\TestFile.xml";
    }

    [TestClass]
    public class ctor : XmlFileStoreTestsBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresFileName()
        {
            new XmlFileStore<BasicSetting>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNameNotRootedException))]
        public void RequiresFileNameIsRooted()
        {
            new XmlFileStore<BasicSetting>(@"a\b\c.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void RequiresFileNameExists()
        {
            new XmlFileStore<BasicSetting>(@"C:\a\b\c.xml");
        }

        [TestMethod]
        public void CreatesXmlFileStore()
        {
            var xmlFileStore = new XmlFileStore<BasicSetting>(TestFileName);
            Assert.AreEqual(TestFileName, xmlFileStore.FileName);
        }
    }

    [TestClass]
    public class Select : XmlFileStoreTestsBase
    {
        [TestMethod]
        public void SelectsBasicSetting()
        {
            Configuration.Load(typeof(Config1)).From(new XmlFileStore<BasicSetting>(TestFileName));
            Assert.AreEqual("Bar", Config1.Foo);
        }

        [SmartConfig]
        static class Config1
        {
             static public string Foo { get; set; }
        }

        [TestMethod]
        public void SelectsBasicSettingWithConfigurationName()
        {
            Configuration
                .Load(typeof(Config2))
                .From(new XmlFileStore<BasicSetting>(TestFileName));
            Assert.AreEqual("Qux", Config2.Foo);
        }

        [SmartConfig]
        [SettingName("baz")]
        static class Config2
        {
            static public string Foo { get; set; }
        }

        [TestMethod]
        public void SelectsCustomSetting()
        {
            Configuration
                .Load(typeof(Config3))
                .WithCustomKey("Environment", "corge")
                .From(new XmlFileStore<CustomTestSetting>(TestFileName));
            Assert.AreEqual("Baaz", Config3.Foo);
        }

        [SmartConfig]
        static class Config3
        {
            static public string Foo { get; set; }
        }
    }

    [TestClass]
    public class Update : XmlFileStoreTestsBase
    {
        [TestMethod]
        public void UpdatesCustomSetting()
        {
            Configuration
                .Load(typeof(Config1))
                .WithCustomKey("Environment", "corge")
                .WithCustomKey("Version", "4.1.5")
                .From(new XmlFileStore<CustomTestSetting>(TestFileName));

            Assert.AreEqual("plugh", Config1.Waldo);

            Config1.Waldo = "foox";
            Configuration.Save(typeof(Config1));


            Config1.Waldo = "fooxxx";
            Configuration.Reload(typeof(Config1));
            Assert.AreEqual("foox", Config1.Waldo);
        }

        [SmartConfig]
        static class Config1
        {
            static public string Waldo { get; set; }
        }

        [TestMethod]
        public void AddsNewSetting()
        {
            Configuration
                .Load(typeof(Config2))
                .From(new XmlFileStore<BasicSetting>(TestFileName));

            Config2.Xyzzy = "thudy";
            Configuration.Save(typeof(Config2));

            Configuration.Reload(typeof(Config2));
            Assert.AreEqual("thudy", Config2.Xyzzy);
        }

        [SmartConfig]
        static class Config2
        {
            [Optional]
            public static string Xyzzy { get; set; } = "thud";
        }

    }
}
