using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.DataStores.Registry.Tests.RegistryPathTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void CreatesRegistryPathWithModelName()
        {
            var registryPath = new RegistryPath(SettingPath.Create(null, "Foo", "Bar", "Baz"));

            Assert.AreEqual(@"Foo\Bar", registryPath.SubKeyName);
            Assert.AreEqual(@"Baz", registryPath.ValueName);
        }

        [TestMethod]
        public void CreatesRegistryPathWithoutModelName()
        {
            var registryPath = new RegistryPath(SettingPath.Create("qux", "Foo", "Bar", "Baz"));

            Assert.AreEqual(@"qux\Foo\Bar", registryPath.SubKeyName);
            Assert.AreEqual(@"Baz", registryPath.ValueName);
        }
    }
}
