using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;

namespace SmartConfig.DataStores.Registry.Tests.Unit
{
    [TestClass]
    public class RegistryUrnTest
    {
        [TestMethod]
        public void ctor_CreateFromSettingUrnWithoutNamespace()
        {
            var registryPath = new RegistryPath(new SettingPath(new[] { "Foo" }));

            registryPath.Namespace.Verify().IsNullOrEmpty();
            registryPath.WeakName.Verify().IsEqual(@"Foo");
        }

        [TestMethod]
        public void ctor_CreateFromSettingUrnWithNamespace()
        {
            var registryPath = new RegistryPath(new SettingPath(new[] { "qux", "Foo", "Bar", "Baz" }));

            registryPath.Namespace.Verify().IsEqual(@"qux\Foo\Bar");
            registryPath.WeakName.Verify().IsEqual(@"Baz");
        }

        [TestMethod]
        public void Parse_Name()
        {
            var registryPath = RegistryPath.Parse("Foo");

            registryPath.Namespace.Verify().IsNullOrEmpty();
            registryPath.WeakName.Verify().IsEqual(@"Foo");
        }

        [TestMethod]
        public void Parse_NameWithNamespace()
        {
            var registryPath = RegistryPath.Parse(@"qux\Foo\Bar\Baz");

            registryPath.Namespace.Verify().IsEqual(@"qux\Foo\Bar");
            registryPath.WeakName.Verify().IsEqual(@"Baz");
        }
    }
}
