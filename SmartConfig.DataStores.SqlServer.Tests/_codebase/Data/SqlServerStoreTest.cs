using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;

namespace SmartConfig.DataStores.SqlServer.Tests.Data
{
    // ReSharper disable InconsistentNaming


    [TestClass]
    public class SqlServerStoreTest
    {
        [TestMethod]
        public void ctor_CreateWithConnectionString()
        {
            var store = new SqlServerStore("foo");
            store.ConnectionString.Validate().IsNotNullOrEmpty();
            store.ConnectionString.Validate().IsEqual("foo");
            store.TableMetadata.Validate().IsNotNull();
        }

        [TestMethod]
        public void ctor_CreateWithConnectionStringName()
        {
            var store = new SqlServerStore("name=foo");
            store.ConnectionString.Validate().IsNotNullOrEmpty();
            store.ConnectionString.Validate().IsEqual("bar");
            store.TableMetadata.Validate().IsNotNull();
        }

        [TestMethod]
        public void ctor_CreateWithCustomSettingTableProperties()
        {
            var store = new SqlServerStore("foo", configure => configure
                    .TableName("qux")
                    .SchemaName("baz")
                    .Column("corge", SqlDbType.Bit, 1)
            );
            store.ConnectionString.Validate().IsNotNullOrEmpty();
            store.ConnectionString.Validate().IsEqual("foo");
            store.TableMetadata.Validate().IsNotNull();
            store.TableMetadata.SchemaName.Validate().IsEqual("baz");
            store.TableMetadata.TableName.Validate().IsEqual("qux");
            store.TableMetadata.Columns["corge"].DbType.Validate().IsEqual(SqlDbType.Bit);
        }

        [TestMethod]
        public void ctor_ConnectionStringNull()
        {
            new Action(() =>
                {
                    new SqlServerStore(null);
                })
                .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void ctor_ConnectionStringEmpty()
        {
            new Action(() =>
                {
                    new SqlServerStore(null);
                })
                .Verify().Throws<ValidationException>();
        }

        [TestMethod]
        public void ctor_ConnectionStringNotFound()
        {
            new Action(() =>
                {
                    new SqlServerStore("name=bar");

                })
                .Verify().Throws<ValidationException>();
        }
    }
}