using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.ObjectConverters;

namespace SmartConfig.Core.Tests
{
    [TestClass]
    public class Global
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Configuration.Converters.Add<JsonConverter>(c =>
            {
                c.ObjectTypes.Add(typeof(List<int>));
                c.ObjectTypes.Add(typeof(double[]));
            });
        }
    }
}
