using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SQLite.Tests.SQLiteStoreTests
{
    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void SelectsBasicSettingByName()
        {
            Configuration
                .Load(typeof(Qux))
                .From(new SQLiteStore<BasicSetting>("name=configdb", "Setting"));

            Assert.AreEqual("Bar", Qux.Foo);
        }

        [SmartConfig]
        static class Qux
        {
            public static string Foo { get; set; }
        }

        [TestMethod]
        public void SelectsCustomSettingByName()
        {
            Configuration
                .Load(typeof(Qux))
                .WithCustomKey("Environment", "SQLiteStore")
                .From(new SQLiteStore<CustomTestSetting>("name=configdb", "Setting"));

            Assert.AreEqual("Bar", Qux.Foo);
        }
    }

    [TestClass]
    public class UpdateTests
    {
        [TestMethod]
        public void UpdatesSettingByName()
        {
            Configuration
                .Load(typeof(Qux))
                .WithCustomKey("Environment", "SQLiteStore")
                .From(new SQLiteStore<CustomTestSetting>("name=configdb", "Setting"));

            Assert.IsNull(Qux.Quak);

            Qux.Quak = "Bux";
            Configuration.Save(typeof(Qux));

            Qux.Quak = "Foob";

            Configuration.Reload(typeof(Qux));

            Assert.AreEqual("Bux", Qux.Quak);
        }

        [SmartConfig]
        static class Qux
        {
            [Optional]
            public static string Quak { get; set; }
        }
    }
}
