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
            var connectionString = new AppConfigRepository().GetConnectionString("configdb");
            using (var connection = new SQLiteConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText  = File.ReadAllText("config.sql").Recode(Encoding.UTF8, Encoding.Default);
                command.ExecuteNonQuery();
            }
        }
    }
}
