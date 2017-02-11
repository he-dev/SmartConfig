using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;

namespace SmartConfig.DataStores.AppConfig.Tests
{
    [TestClass]
    public class ConnectionStringsStoreTest
    {
        [TestMethod]
        public void Load_SimpleConfig()
        {
            //Configuration.Builder.From(new ConnectionStringsStore()).Select(typeof(FullConfig3));

            //FullConfig3.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foox");
            //FullConfig3.NestedConfig.StringSetting.Verify().IsEqual("Barx");
            //FullConfig3.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }

        [TestMethod]
        public void Load_ConfigWithNameAsPath()
        {
            //Configuration.Builder.From(new ConnectionStringsStore()).Select(typeof(FullConfig4));

            //FullConfig4.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Fooxy");
            //FullConfig4.NestedConfig.StringSetting.Verify().IsEqual("Barxy");
            //FullConfig4.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }
    }    
}