using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            DataStore = new SQLiteStore("name=configdb", builder => builder.TableName("Setting1"));
        }      
    }
}
