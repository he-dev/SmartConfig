using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Data;
using Reusable.Data.Annotations;
using Reusable.Fuse.Testing;
using Reusable.Fuse;
using SmartConfig.Data;
using SmartConfig.Data.Annotations;
using SmartConfig.DataStores.SqlServer;
using SmartConfig.DataStores.Tests.Common;
using SmartConfig.DataStores.Tests.Data;

namespace SmartConfig.DataStores.SqlServer.Tests
{
    // ReSharper disable InconsistentNaming

    [TestClass]
    public class ConfigurationTest : ConfigurationTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            DataStores = new Dictionary<Type, DataStore>
            {
                [typeof(TestConfig1)] = new SqlServerStore("name=SmartConfigTest", configure => configure.TableName("Setting1"))
            };

            ResetData();
        }

        private static void ResetData()
        {
            //AppDomain.CurrentDomain.SetData("DataDirectory", Environment.CurrentDirectory);

            // Insert test data.
            var connectionString = new AppConfigRepository().GetConnectionString("SmartConfigTest");

            SqlConnection OpenSqlConnection()
            {
                var sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                return sqlConnection;
            }

            using (var sqlConnection = OpenSqlConnection())
            using (var sqlCommand = sqlConnection.CreateCommand())
            using (var transaction = sqlConnection.BeginTransaction())
            {
                sqlCommand.Transaction = transaction;
                try
                {
                    sqlCommand.CommandText = ResourceReader.ReadEmbededResource<ConfigurationTest, ConfigurationTest>("Resources.CreateSettingTables.sql");
                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.CommandText = ResourceReader.ReadEmbededResource<ConfigurationTest, ConfigurationTest>("Resources.TruncateSettingTables.sql");
                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.CommandText = ResourceReader.ReadEmbededResource<ConfigurationTest, ConfigurationTest>("Resources.InsertSetting1.sql");
                    sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
                    sqlCommand.Parameters.Add("@Value", SqlDbType.NVarChar, -1);
                    var testSettings = TestSettingFactory.CreateTestSettings().ToList();
                    foreach (var testSetting in testSettings)
                    {
                        sqlCommand.Parameters["@Name"].Value = testSetting.Name;
                        sqlCommand.Parameters["@Value"].Value = testSetting.Value;
                        sqlCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }
    }
}