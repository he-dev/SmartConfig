using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Data;

namespace SmartConfig.DataStores.SqlServer.Tests
{
    [TestClass]
    public class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            //AppDomain.CurrentDomain.SetData("DataDirectory", Environment.CurrentDirectory);

            // Insert test data.
            var connectionString = new AppConfigRepository().GetConnectionString("SmartConfigTest");
            using (var sqLiteConnection = new SqlConnection(connectionString))
            using (var sqLiteCommand = sqLiteConnection.CreateCommand())
            {
                sqLiteConnection.Open();

                sqLiteCommand.CommandText  = File.ReadAllText("config.sql");
                sqLiteCommand.ExecuteNonQuery();
            }
        }
    }
}
