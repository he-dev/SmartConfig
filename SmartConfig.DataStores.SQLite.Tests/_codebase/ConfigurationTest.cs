using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Data;
using SmartConfig.Data;
using SmartConfig.DataStores.Tests.Common;

// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SmartConfig.DataStores.SQLite.Tests
{
    using SQLite;

    [TestClass]
    public class ConfigurationTest : ConfigurationTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        { 
            DataStores = new Dictionary<Type, DataStore>
            {
                [typeof(TestConfig1)] = new SQLiteStore("name=configdb", builder => builder.TableName("Setting1"))
            };

            ResetData();
        }

        private static void ResetData()
        {
            // Insert test data.
            var connectionString = new AppConfigRepository().GetConnectionString("configdb");
            using (var sqLiteConnection = new SQLiteConnection(connectionString))
            using (var sqLiteCommand = sqLiteConnection.CreateCommand())
            {
                sqLiteConnection.Open();

                // Encode query for sqlite or otherwise the utf8 will be broken.
                sqLiteCommand.CommandText = File.ReadAllText("config.sql").Recode(Encoding.UTF8, Encoding.Default);
                sqLiteCommand.ExecuteNonQuery();
            }
        }
    }
}
