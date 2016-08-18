using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.Frameworks.InlineValidation;
using SmartUtilities.Frameworks.InlineValidation.Testing;

// ReSharper disable InconsistentNaming


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
            store.SettingTableConfiguration.Validate().IsNotNull();
        }

        [TestMethod]
        public void CreateWithConnectionStringName()
        {
            var store = new SqlServerStore("name=foo");
            store.ConnectionString.Validate().IsNotNullOrEmpty();
            store.ConnectionString.Validate().IsEqual("bar");
            store.SettingTableConfiguration.Validate().IsNotNull();
        }

        [TestMethod]
        public void CreateWithCustomSettingTableProperties()
        {
            var store = new SqlServerStore("foo", configure => configure
                .TableName("qux")
                .SchemaName("baz")
                .Column("corge", SqlDbType.Bit, 1)
            );
            store.ConnectionString.Validate().IsNotNullOrEmpty();
            store.ConnectionString.Validate().IsEqual("foo");
            store.SettingTableConfiguration.Validate().IsNotNull();
            store.SettingTableConfiguration.SchemaName.Validate().IsEqual("baz");
            store.SettingTableConfiguration.TableName.Validate().IsEqual("qux");
            store.SettingTableConfiguration.Columns["corge"].DbType.Validate().IsEqual(SqlDbType.Bit);
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
            new Action(() =>
            {
                new SqlServerStore(null);
            })
            .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void ConnectionStringEmpty()
        {
            new Action(() =>
            {
                new SqlServerStore(null);
            })
            .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void ConnectionStringNotFound()
        {
            new Action(() =>
            {
                new SqlServerStore("name=bar");
                
            })
            .Verify().Throws<ValidationException>();
        }
    }
}