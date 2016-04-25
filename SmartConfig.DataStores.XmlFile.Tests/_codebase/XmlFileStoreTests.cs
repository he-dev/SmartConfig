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
using SmartUtilities.ObjectConverters.DataAnnotations;

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
        private static class Config1
        {
             public static string Foo { get; set; }
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
        [CustomName("baz")]
        private static class Config2
        {
            public static string Foo { get; set; }
        }

        [TestMethod]
        public void SelectsCustomSetting()
        {
            Configuration
                .Load(typeof(Config3))
                .From(new XmlFileStore<CustomTestSetting>(TestFileName), dataStore =>
                {
                    dataStore.SetCustomKey("Environment", "corge");
                });
            Assert.AreEqual("Baaz", Config3.Foo);
        }

        [SmartConfig]
        private static class Config3
        {
            public static string Foo { get; set; }
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
                .From(new XmlFileStore<CustomTestSetting>(TestFileName), dataStore =>
                {
                    dataStore.SetCustomKey("Environment", "corge");
                    dataStore.SetCustomKey("Version", "4.1.5");
                });

            Assert.AreEqual("plugh", Config1.Waldo);

            Config1.Waldo = "foox";
            Configuration.Save(typeof(Config1));


            Config1.Waldo = "fooxxx";
            Configuration.Reload(typeof(Config1));
            Assert.AreEqual("foox", Config1.Waldo);
        }

        [SmartConfig]
        private static class Config1
        {
            public static string Waldo { get; set; }
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
        private static class Config2
        {
            [Optional]
            public static string Xyzzy { get; set; } = "thud";
        }

    }
}
