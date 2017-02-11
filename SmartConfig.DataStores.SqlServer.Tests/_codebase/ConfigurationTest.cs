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
                [typeof(TestConfig1)] = new SqlServerStore("name=SmartConfigTest", configure => 
                    configure
                        .TableName("Setting1")),
                [typeof(TestConfig3)] = new SqlServerStore("name=SmartConfigTest", configure => 
                    configure
                        .TableName("Setting3")
                        .Column("Environment", SqlDbType.NVarChar, 50)
                        .Column("Config", SqlDbType.NVarChar, 50))
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
                    sqlCommand.CommandText = ResourceReader.ReadEmbeddedResource<ConfigurationTest>("Resources.CreateSettingTables.sql");
                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.CommandText = ResourceReader.ReadEmbeddedResource<ConfigurationTest>("Resources.TruncateSettingTables.sql");
                    sqlCommand.ExecuteNonQuery();

                    InsertSetting1(sqlCommand);
                    InsertSetting3(sqlCommand);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        private static void InsertSetting1(SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = ResourceReader.ReadEmbeddedResource<ConfigurationTest>("Resources.InsertSetting1.sql");
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@Value", SqlDbType.NVarChar, -1);

            foreach (var testSetting in TestSettingFactory.CreateTestSettings1())
            {
                sqlCommand.Parameters["@Name"].Value = testSetting.Name;
                sqlCommand.Parameters["@Value"].Value = testSetting.Value;
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void InsertSetting3(SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = ResourceReader.ReadEmbeddedResource<ConfigurationTest>("Resources.InsertSetting3.sql");
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@Value", SqlDbType.NVarChar, -1);
            sqlCommand.Parameters.Add("@Environment", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@Config", SqlDbType.NVarChar, 50);

            foreach (var testSetting in TestSettingFactory.CreateTestSettings3())
            {
                sqlCommand.Parameters["@Name"].Value = testSetting.Name;
                sqlCommand.Parameters["@Value"].Value = testSetting.Value;
                sqlCommand.Parameters["@Environment"].Value = testSetting.Tags["Environment"];
                sqlCommand.Parameters["@Config"].Value = testSetting.Tags["Config"];
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}