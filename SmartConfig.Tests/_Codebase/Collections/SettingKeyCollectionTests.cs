using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Collections.SettingKeyCollectionTests
{
    [TestClass]
    public class NameKeyTests
    {
        [TestMethod]
        public void GetsNameKey()
        {
            var settingKeyCollection = new CompoundSettingKey(
                new SimpleSettingKey("foo", new SettingPath(null, "bar")),
                Enumerable.Empty<SimpleSettingKey>());
            var expectedNameKey = new NameKey(new SimpleSettingKey("foo", new SettingPath(null, "bar")));

            Assert.IsTrue(settingKeyCollection.NameKey == expectedNameKey);
        }
    }

    [TestClass]
    public class CustomKeysTests
    {
        [TestMethod]
        public void GetsCustomKeys()
        {
            var settingKeyCollection = new CompoundSettingKey(
                new SimpleSettingKey("foo", new SettingPath(null, "bar")),
                new[]
                {
                    new SimpleSettingKey("baz", "qux"),
                    new SimpleSettingKey("buq", "bax")
                });

            var expectedCustomKeys = new[]
            {
                new SimpleSettingKey("baz", "qux"),
                new SimpleSettingKey("buq", "bax")
            };

            CollectionAssert.AreEqual(expectedCustomKeys, settingKeyCollection.CustomKeys.ToList());
        }
    }
}
