using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Testing;
using Reusable.Validations;
using SmartConfig.Data;

namespace SmartConfig.DataStores.Registry.Tests.Unit
{
    [TestClass]
    public class RegistryUrnTest
    {
        [TestMethod]
        public void ctor_CreateFromSettingUrnWithoutNamespace()
        {
            var registryPath = new RegistryUrn(new SettingUrn(new[] { "Foo" }));

            registryPath.Namespace.Verify().IsNullOrEmpty();
            registryPath.WeakName.Verify().IsEqual(@"Foo");
        }

        [TestMethod]
        public void ctor_CreateFromSettingUrnWithNamespace()
        {
            var registryPath = new RegistryUrn(new SettingUrn(new[] { "qux", "Foo", "Bar", "Baz" }));

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
