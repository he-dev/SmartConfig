using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Data;

namespace SmartConfig.DataStores.SQLite.Tests
{
    [TestClass]
    public class TestAssemblyInitialize
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            // Insert test data.
            var connectionString = new AppConfigRepository().GetConnectionString("configdb");
            using (var sqLiteConnection = new SQLiteConnection(connectionString))
            using (var sqLiteCommand = sqLiteConnection.CreateCommand())
            {
                sqLiteConnection.Open();

                // Encode query for sqlite or otherwise the utf8 will be broken.
                sqLiteCommand.CommandText  = File.ReadAllText("config.sql").Recode(Encoding.UTF8, Encoding.Default);
                sqLiteCommand.ExecuteNonQuery();
            }
        }
    }
}
