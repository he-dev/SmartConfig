using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Paths;

namespace SmartConfig.Tests.Paths
{
    [TestClass]
    public class RegistryPathTests
    {
        [TestMethod]
        public void GetsSubKeyName()
        {
            Assert.AreEqual(
                @"Foo\Bar",
                new RegistryPath(new SettingPath(null, "Foo", "Bar", "Baz")).SubKeyName);

            Assert.AreEqual(
                @"qux\Foo\Bar",
                new RegistryPath(new SettingPath("qux", "Foo", "Bar", "Baz")).SubKeyName);
        }

        [TestMethod]
        public void GetsValueName()
        {
            Assert.AreEqual(
                @"Baz",
                new RegistryPath(new SettingPath(null, "Foo", "Bar", "Baz")).ValueName);

            Assert.AreEqual(
                @"Baz",
                new RegistryPath(new SettingPath("qux", "Foo", "Bar", "Baz")).ValueName);
        }
    }
}
