using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.DataStores.SqlServer.Tests
{
    [TestClass]
    public class Global
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Database.SetInitializer(new TestDbInitializer());
        }
    }
}
