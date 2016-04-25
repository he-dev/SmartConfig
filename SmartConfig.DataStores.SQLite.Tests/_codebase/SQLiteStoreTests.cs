using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.ObjectConverters.DataAnnotations;

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
        private static class Qux
        {
            public static string Foo { get; set; }
        }

        [TestMethod]
        public void SelectsCustomSettingByName()
        {
            Configuration
                .Load(typeof(Qux))
                .From(new SQLiteStore<CustomTestSetting>("name=configdb", "Setting"), dataStore =>
                {
                    dataStore.SetCustomKey("Environment", "SQLiteStore");
                });

            Assert.AreEqual("Bar", Qux.Foo);
        }

        [TestMethod]
        public void SelectsUnicodeStrings()
        {
            Configuration
                .Load(typeof(UnicodeStrings))
                .From(new SQLiteStore<CustomTestSetting>("name=configdb", "Setting"), dataStore =>
                {
                    dataStore.SetCustomKey("Environment", "SQLiteStore");
                });

            Assert.AreEqual("äöüß", UnicodeStrings.DE);
            Assert.AreEqual("ąęśćżźó", UnicodeStrings.PL);
        }

        [SmartConfig]
        private static class UnicodeStrings
        {
            public static string DE { get; set; }
            public static string PL { get; set; }
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
                .From(new SQLiteStore<CustomTestSetting>("name=configdb", "Setting"), dataStore =>
                {
                    dataStore.SetCustomKey("Environment", "SQLiteStore");
                });

            Assert.IsNull(Qux.Quak);

            var now = DateTime.UtcNow.ToLongDateString();

            Qux.Quak = now;
            Configuration.Save(typeof(Qux));

            Qux.Quak = "Foob";

            Configuration.Reload(typeof(Qux));

            Assert.AreEqual(now, Qux.Quak);
        }

        [SmartConfig]
        private static class Qux
        {
            [Optional]
            public static string Quak { get; set; }
        }
    }
}
