using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace SmartConfig.DataStores.SqlServer.Tests.Unit.SqlServerStore.Positive
{
    using SqlServer;

    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void CreateWithConnectionString()
        {
            var store = new SqlServerStore("foo");
            store.ConnectionString.Validate().IsNotNullOrEmpty();
            store.ConnectionString.Validate().IsEqual("foo");
            store.SettingTableProperties.Validate().IsNotNull();
        }

        [TestMethod]
        public void CreateWithConnectionStringName()
        {
            var store = new SqlServerStore("name=foo");
            store.ConnectionString.Validate().IsNotNullOrEmpty();
            store.ConnectionString.Validate().IsEqual("bar");
            store.SettingTableProperties.Validate().IsNotNull();
        }

        [TestMethod]
        public void CreateWithCustomSettingTableProperties()
        {
            var store = new SqlServerStore("foo", builder => builder
                .TableName("qux")
                .SchemaName("baz")
                .ColumnProperties("corge", SqlDbType.Bit));
            store.ConnectionString.Validate().IsNotNullOrEmpty();
            store.ConnectionString.Validate().IsEqual("foo");
            store.SettingTableProperties.Validate().IsNotNull();
            store.SettingTableProperties.SchemaName.Validate().IsEqual("baz");
            store.SettingTableProperties.TableName.Validate().IsEqual("qux");
            store.SettingTableProperties.SqlDbTypes["corge"].Validate().IsEqual(SqlDbType.Bit);
        }
    }
}

namespace SmartConfig.DataStores.SqlServer.Tests.Unit.SqlServerStore.Negative
{
    using SqlServer;

    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void ConnectionStringNull()
        {
            new Action(() => new SqlServerStore(null)).Validate().Throws<ValidationException>();
        }

        [TestMethod]
        public void ConnectionStringEmpty()
        {
            new Action(() => new SqlServerStore(null)).Validate().Throws<ValidationException>();
        }

        [TestMethod]
        public void ConnectionStringNotFound()
        {
            new Action(() => new SqlServerStore("name=bar")).Validate().Throws<ValidationException>();
        }
    }
}