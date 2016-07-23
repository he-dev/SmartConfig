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
            var registryPath = new RegistryPath(new SettingPath(new[] { "Foo", "Bar", "Baz" }));

            registryPath.SettingNamespace.Verify().IsEqual(@"Foo\Bar");
            registryPath.SettingName.Verify().IsEqual(@"Baz");
        }

        [TestMethod]
        public void CreateWithConfigurationName()
        {
            var registryPath = new RegistryPath(new SettingPath(new[] { "qux", "Foo", "Bar", "Baz" }));

            registryPath.SettingNamespace.Verify().IsEqual(@"qux\Foo\Bar");
            registryPath.SettingName.Verify().IsEqual(@"Baz");
        }
    }
}
