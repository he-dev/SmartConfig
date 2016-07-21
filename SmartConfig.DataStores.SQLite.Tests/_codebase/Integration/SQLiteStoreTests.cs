using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

namespace SmartConfig.DataStores.SQLite.Tests.Integration.SQLiteStore.Positive
{
    using SQLite;
    using TestConfigs;

    [TestClass]
    public class GetSettings
    {
        [TestMethod]
        public void GetSettingsByName()
        {
            Configuration.Load
                .From(new SQLiteStore("name=configdb"))
                .Select(typeof(TestConfig1));

            TestConfig1.Foo.Verify("TestConfig1.Foo").IsNotNullOrEmpty().IsEqual("Bar");
        }

       

        [TestMethod]
        public void GetSettingsByNameAndNamespace()
        {
            Configuration.Load
                .From(new SQLiteStore("name=configdb"))
                .Where("Environment", "Baz")
                .Select(typeof(TestConfig2));

            TestConfig2.Bar.Verify("TestConfig2.Bar").IsNotNullOrEmpty().IsEqual("Qux");
        }

        [TestMethod]
        public void SelectsUnicodeStrings()
        {
            Configuration.Load
                .From(new SQLiteStore("name=configdb"))
                .Where("Environment", "UTF8")
                .Select(typeof(UnicodeStrings));

            UnicodeStrings.DE.Verify("UnicodeStrings.DE").IsNotNullOrEmpty().IsEqual("äöüß");
            UnicodeStrings.PL.Verify("UnicodeStrings.PL").IsNotNullOrEmpty().IsEqual("ąęśćżźó");
        }
    }

    [TestClass]
    public class UpdateTests
    {
        [TestMethod]
        public void UpdatesSettingByName()
        {
            Configuration.Load
                .From(new SQLiteStore("name=configdb"))
                .Where("Environment", "Corge")
                .Select(typeof(TestConfig3));

            var rnd = new Random((int)DateTime.UtcNow.Ticks);
            var newValue = $"{nameof(TestConfig3.Qux)}{rnd.Next(101)}";

            TestConfig3.Qux = newValue;

            Configuration.Save(typeof(TestConfig3));

            TestConfig3.Qux = null;

            Configuration.Reload(typeof(TestConfig3));

            TestConfig3.Qux.Verify("TestConfig3.Qux").IsNotNullOrEmpty().IsEqual(newValue);
        }


    }
}

namespace SmartConfig.DataStores.SQLite.Tests.Integration.SQLiteStore.Positive.TestConfigs
{
    [SmartConfig]
    internal static class TestConfig1
    {
        public static string Foo { get; set; }
    }

    [SmartConfig]
    internal static class TestConfig2
    {
        public static string Bar { get; set; }
    }

    [SmartConfig]
    internal static class TestConfig3
    {
        [Optional]
        public static string Qux { get; set; }
    }

    [SmartConfig]
    internal static class UnicodeStrings
    {
        public static string DE { get; set; }
        public static string PL { get; set; }
    }

    [SmartConfig]
    internal static class Qux
    {
        [Optional]
        public static string Quak { get; set; }
    }
}