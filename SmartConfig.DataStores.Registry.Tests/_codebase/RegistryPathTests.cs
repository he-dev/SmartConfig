using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.DataStores.Registry.Tests.RegistryPathTests
{
    [TestClass]
    public class ConstructorTests
    {
        [TestMethod]
        public void CreatesRegistryPathWithDefaultNames()
        {
            var registryPath = new RegistryPath(SettingPath.Create(null, "Foo", "Bar", "Baz"));

            Assert.AreEqual(@"Foo\Bar", registryPath.SubKeyName);
            Assert.AreEqual(@"Baz", registryPath.ValueName);
        }

        [TestMethod]
        public void CreatesRegistryPathWithCustomNames()
        {
            var registryPath = new RegistryPath(SettingPath.Create("qux", "Foo", "Bar", "Baz"));

            Assert.AreEqual(@"qux\Foo\Bar", registryPath.SubKeyName);
            Assert.AreEqual(@"Baz", registryPath.ValueName);
        }
    }
}
