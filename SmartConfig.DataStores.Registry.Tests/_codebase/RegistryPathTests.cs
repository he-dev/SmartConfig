using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Testing;
using Reusable.Validations;
using SmartConfig.Data;

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
            var registryPath = new RegistryPath(new SettingUrn(new[] { "Foo" }));

            registryPath.Namespace.Verify().IsNullOrEmpty();
            registryPath.Name.Verify().IsEqual(@"Foo");
        }

        [TestMethod]
        public void CreateFromNameAndNamespace()
        {
            var registryPath = new RegistryPath(new SettingUrn(new[] { "qux", "Foo", "Bar", "Baz" }));

            registryPath.Namespace.Verify().IsEqual(@"qux\Foo\Bar");
            registryPath.Name.Verify().IsEqual(@"Baz");
        }
    }
}
