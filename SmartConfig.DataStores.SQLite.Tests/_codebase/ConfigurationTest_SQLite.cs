using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Data;
using SmartConfig.Data;
using SmartConfig.DataStores.Tests.Common;
using SmartConfig.DataStores.Tests.Data;

// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SmartConfig.DataStores.SQLite.Tests
{
    using SQLite;

    [TestClass]
    public class ConfigurationTest_SQLite : ConfigurationTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ConfigInfos = new[]
            {
                new ConfigInfo
                {
                    DataStore = new SQLiteStore("name=configdb", builder => builder.TableName("Setting1")),
                    Tags = new Dictionary<string, object>(),
                    ConfigType = typeof(TestConfig)
                },
                new ConfigInfo
                {
                    DataStore = new SQLiteStore("name=configdb", configure =>
                        configure
                            .TableName("Setting3")
                            .Column("Environment", DbType.String, 50)
                            .Column("Config", DbType.String, 50)
                    ),
                    Tags =
                    {
                        ["Environment"] = "Test",
                        ["Config"]= $"{nameof(TestConfig)}3"
                    },
                    ConfigType = typeof(TestConfig)
                }
            };

            ResetData();
        }

        private static void ResetData()
        {
            // Insert test data.
            var connectionString = new AppConfigRepository().GetConnectionString("configdb");

            SQLiteConnection OpenSQLiteConnection()
            {
                var sqLiteConnection = new SQLiteConnection(connectionString);
                sqLiteConnection.Open();
                return sqLiteConnection;
            }

            using (var connection = OpenSQLiteConnection())
            using (var command = connection.CreateCommand())
            using (var transaction = connection.BeginTransaction())
            {
                command.Transaction = transaction;
                try
                {
                    command.CommandText = ResourceReader.ReadEmbeddedResource<ConfigurationTest_SQLite>("Resources.CreateSettingTables.sql");
                    command.ExecuteNonQuery();

                    //command.CommandText = ResourceReader.ReadEmbeddedResource<ConfigurationTest>("Resources.TruncateSettingTables.sql");
                    //command.ExecuteNonQuery();

                    // Insert test data.
                    InsertSetting1(command);
                    InsertSetting3(command);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        private static void InsertSetting1(SQLiteCommand command)
        {
            command.CommandText = ResourceReader.ReadEmbeddedResource<ConfigurationTest_SQLite>("Resources.InsertSetting1.sql");
            command.Parameters.Clear();
            command.Parameters.Add("@Name", DbType.String, 50);
            command.Parameters.Add("@Value", DbType.String, -1);

            foreach (var testSetting in TestSettingFactory.CreateTestSettings1())
            {
                command.Parameters["@Name"].Value = testSetting.Name;
                command.Parameters["@Value"].Value = testSetting.Value.Recode(Encoding.UTF8, Encoding.Default);
                command.ExecuteNonQuery();
            }
        }

        private static void InsertSetting3(SQLiteCommand command)
        {
            command.CommandText = ResourceReader.ReadEmbeddedResource<ConfigurationTest_SQLite>("Resources.InsertSetting3.sql");
            command.Parameters.Clear();
            command.Parameters.Add("@Name", DbType.String, 50);
            command.Parameters.Add("@Value", DbType.String, -1);
            command.Parameters.Add("@Environment", DbType.String, 50);
            command.Parameters.Add("@Config", DbType.String, 50);

            foreach (var testSetting in TestSettingFactory.CreateTestSettings3())
            {
                command.Parameters["@Name"].Value = testSetting.Name;
                command.Parameters["@Value"].Value = testSetting.Value.Recode(Encoding.UTF8, Encoding.Default);
                command.Parameters["@Environment"].Value = testSetting.Tags["Environment"];
                command.Parameters["@Config"].Value = testSetting.Tags["Config"];
                command.ExecuteNonQuery();
            }
        }
    }
}
