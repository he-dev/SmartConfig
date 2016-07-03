using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;

namespace SmartConfig.DataStores.AppConfig.Tests.AppConfigStoreTests
{
    [TestClass]
    public class Select
    {
        [TestMethod]
        public void SelectsAppSettingWithoutConfigurationName()
        {
            Configuration.Load(typeof(Config1)).From(new AppConfigStore());
            Assert.AreEqual("Bar", Config1.appSettings.Foo);
        }

        [SmartConfig]
        private static class Config1
        {
            public static class appSettings
            {
                public static string Foo { get; set; }
            }
        }

        [TestMethod]
        public void SelectsAppSettingWithConfigurationName()
        {
            Configuration.Load(typeof(Config2)).From(new AppConfigStore());
            Assert.AreEqual("Barx", Config2.appSettings.Foo);
        }

        [SmartConfig]
        [CustomName("baz")]
        private static class Config2
        {
            public static class appSettings
            {
                public static string Foo { get; set; }
            }
        }

        [TestMethod]
        public void SelectsConnectionStringWithoutConfigurationName()
        {
            Configuration.Load(typeof(Config3)).From(new AppConfigStore());
            Assert.AreEqual("baz", Config3.connectionStrings.Foo.Bar);
        }

        [SmartConfig]
        private static class Config3
        {
            public static class connectionStrings
            {
                public static class Foo
                {
                    public static string Bar { get; set; }
                }
            }
        }

        [TestMethod]
        public void SelectsonnectionStringByNameAndConfigName()
        {
            Configuration.Load(typeof(Config4)).From(new AppConfigStore());
            Assert.AreEqual("quux", Config4.connectionStrings.Foo.Bar);
        }

        [SmartConfig]
        [CustomName("bax")]
        private static class Config4
        {
            public static class connectionStrings
            {
                public static class Foo
                {
                    public static string Bar { get; set; }
                }
            }
        }

    }

    [TestClass]
    public class Update
    {
        [TestMethod]
        public void UpdatesAppSettingWithoutConfigurationName()
        {
            Configuration.Load(typeof(Config1)).From(new AppConfigStore());
            Assert.AreEqual("Quax", Config1.appSettings.Qux);

            Config1.appSettings.Qux = "Baazx";
            Configuration.Save(typeof(Config1));

            Configuration.Load(typeof(Config1)).From(new AppConfigStore());
            Assert.AreEqual("Baazx", Config1.appSettings.Qux);
        }

        [SmartConfig]
        private static class Config1
        {
            public static class appSettings
            {
                public static string Qux { get; set; }
            }
        }
    }
}
