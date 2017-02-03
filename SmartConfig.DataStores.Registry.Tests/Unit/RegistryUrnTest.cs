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
            var registryPath = new RegistryUrn(new SettingPath(new[] { "Foo" }));

            registryPath.Namespace.Verify().IsNullOrEmpty();
            registryPath.WeakName.Verify().IsEqual(@"Foo");
        }

        [TestMethod]
        public void ctor_CreateFromSettingUrnWithNamespace()
        {
            var registryPath = new RegistryUrn(new SettingPath(new[] { "qux", "Foo", "Bar", "Baz" }));

            registryPath.Namespace.Verify().IsEqual(@"qux\Foo\Bar");
            registryPath.WeakName.Verify().IsEqual(@"Baz");
        }

        [TestMethod]
        public void Parse_Name()
        {
            var registryPath = RegistryUrn.Parse("Foo");

            registryPath.Namespace.Verify().IsNullOrEmpty();
            registryPath.WeakName.Verify().IsEqual(@"Foo");
        }

        [TestMethod]
        public void Parse_NameWithNamespace()
        {
            var registryPath = RegistryUrn.Parse(@"qux\Foo\Bar\Baz");

            registryPath.Namespace.Verify().IsEqual(@"qux\Foo\Bar");
            registryPath.WeakName.Verify().IsEqual(@"Baz");
        }
    }
}
