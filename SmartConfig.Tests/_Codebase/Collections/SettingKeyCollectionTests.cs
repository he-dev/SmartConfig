using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Collections.SettingKeyCollectionTests
{
    [TestClass]
    public class NameKeyTests
    {
        [TestMethod]
        public void GetsNameKey()
        {
            var settingKeyCollection = new SettingKeyCollection(
                new SettingKey("foo", new SettingPath(null, "bar")),
                Enumerable.Empty<SettingKey>());
            var expectedNameKey = new NameKey(new SettingKey("foo", new SettingPath(null, "bar")));

            Assert.IsTrue(settingKeyCollection.NameKey == expectedNameKey);
        }
    }

    [TestClass]
    public class CustomKeysTests
    {
        [TestMethod]
        public void GetsCustomKeys()
        {
            var settingKeyCollection = new SettingKeyCollection(
                new SettingKey("foo", new SettingPath(null, "bar")),
                new[]
                {
                    new SettingKey("baz", "qux"),
                    new SettingKey("buq", "bax")
                });

            var expectedCustomKeys = new[]
            {
                new SettingKey("baz", "qux"),
                new SettingKey("buq", "bax")
            };

            CollectionAssert.AreEqual(expectedCustomKeys, settingKeyCollection.CustomKeys.ToList());
        }
    }
}
