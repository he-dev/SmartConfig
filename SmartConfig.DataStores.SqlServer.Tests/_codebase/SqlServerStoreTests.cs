﻿using System;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartUtilities.UnitTesting;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace SmartConfig.DataStores.SqlServer.Tests.SqlServerStoreTests
{
    [TestClass]
    public class SqlServerStoreTestsBase
    {
        protected const string TestTableName = "TestSetting";
        protected const string ConnectionStringName = "TestDb";
        protected readonly string ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            using (var context = new SqlServerContext<CustomTestSetting>(connectionString, TestTableName))
            {
                context.Database.Initialize(true);
            }
        }
    }

    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void RequiresConnectionString()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new SqlServerStore<BasicSetting>(null, null);
            }, ex => Assert.AreEqual("connectionString", ex.ParamName), Assert.Fail);
        }

        [TestMethod]
        public void RequiresTableName()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new SqlServerStore<BasicSetting>("abc", null);
            }, ex => Assert.AreEqual("settingsTableName", ex.ParamName), Assert.Fail);
        }
    }

    [TestClass]
    public class Select : SqlServerStoreTestsBase
    {
        [TestMethod]
        public void SelectsBasicSettingWithoutModelName()
        {
            Configuration.Load(typeof(Config1)).From(new SqlServerStore<BasicSetting>(ConnectionString, TestTableName));
            Assert.AreEqual("Grault", Config1.Foo);
        }

        [SmartConfig]
        static class Config1
        {
            static public string Foo { get; set; }
        }

        // ------------------------------------------------

        [TestMethod]
        public void SelectsBasicSettingWithModelName()
        {
            Configuration.Load(typeof(Config2)).From(new SqlServerStore<BasicSetting>(ConnectionString, TestTableName));
            Assert.AreEqual("Fred", Config2.Foo);
        }

        [SmartConfig]
        [SettingName("baz")]
        static class Config2
        {
            static public string Foo { get; set; }
        }

        // ------------------------------------------------

        [TestMethod]
        public void SelectsCustomSettingWithoutModelName()
        {
            Configuration
                .Load(typeof(Config3))
                .WithCustomKey("Environment", "corge")
                .WithCustomKey("Version", "1.3.0")
                .From(new SqlServerStore<CustomTestSetting>(ConnectionString, TestTableName));
            Assert.AreEqual("Plugh", Config3.Foo);
        }

        [SmartConfig]
        static class Config3
        {
            static public string Foo { get; set; }
        }
    }

    [TestClass]
    public class Update : SqlServerStoreTestsBase
    {
        [TestMethod]
        public void UpdatesCustomSetting()
        {
            Configuration
                .Load(typeof(Config1))
                .WithCustomKey("Environment", "corge")
                .WithCustomKey("Version", "2.5.0")
                .From(new SqlServerStore<CustomTestSetting>(ConnectionString, TestTableName));
            Assert.AreEqual("Baz", Config1.Bar);

            Config1.Bar = "Quuux";
            Configuration.Save(typeof(Config1));

            Config1.Bar = "Quuuxxx";
            Configuration.Reload(typeof(Config1));
            Assert.AreEqual("Quuux", Config1.Bar);
        }

        [SmartConfig]
        static class Config1
        {
            static public string Bar { get; set; }
        }
    }
}