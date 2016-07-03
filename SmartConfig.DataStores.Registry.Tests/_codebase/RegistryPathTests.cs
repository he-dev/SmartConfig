using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.ValidationExtensions.Testing;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig.DataStores.Registry.Tests.Unit.RegistryPath.Positive
{
    using Registry;

    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void CreateWithoutConfigurationName()
        {
            var registryPath = new RegistryPath(new SettingPath(null, "Foo", "Bar", "Baz"));

            registryPath.SubKeyName.Verify().IsEqual(@"Foo\Bar");
            registryPath.ValueName.Verify().IsEqual(@"Baz");
        }

        [TestMethod]
        public void CreateWithConfigurationName()
        {
            var registryPath = new RegistryPath(new SettingPath("qux", "Foo", "Bar", "Baz"));

            registryPath.SubKeyName.Verify().IsEqual(@"qux\Foo\Bar");
            registryPath.ValueName.Verify().IsEqual(@"Baz");
        }
    }
}
