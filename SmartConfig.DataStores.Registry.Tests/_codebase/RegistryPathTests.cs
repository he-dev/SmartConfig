using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.ValidationExtensions.Testing;
using SmartUtilities.ValidationExtensions;
// ReSharper disable CheckNamespace

namespace SmartConfig.DataStores.Registry.Tests.Unit.RegistryPath.Positive
{
    using Registry;

    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void CreateFromName()
        {
            var registryPath = new RegistryPath(new SettingPath(new[] { "Foo" }));

            registryPath.SettingNamespace.Verify().IsNullOrEmpty();
            registryPath.SettingName.Verify().IsEqual(@"Foo");
        }

        [TestMethod]
        public void CreateFromNameAndNamespace()
        {
            var registryPath = new RegistryPath(new SettingPath(new[] { "qux", "Foo", "Bar", "Baz" }));

            registryPath.SettingNamespace.Verify().IsEqual(@"qux\Foo\Bar");
            registryPath.SettingName.Verify().IsEqual(@"Baz");
        }
    }
}
