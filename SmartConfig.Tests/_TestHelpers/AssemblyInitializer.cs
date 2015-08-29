using System.Data.Entity;
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
            Database.SetInitializer(new TestDbInitializer());
        }
    }
}
