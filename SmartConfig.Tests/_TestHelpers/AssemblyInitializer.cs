using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Tests.Data;

namespace SmartConfig.Tests
{
    [TestClass]
    public class Global
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            System.Data.Entity.Database.SetInitializer(new TestDbInitializer());
        }
    }
}
