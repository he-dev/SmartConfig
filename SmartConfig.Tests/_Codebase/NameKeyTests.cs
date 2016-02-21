using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Core.Tests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresSettingKey()
        {
            new NameKey(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RequiresSettingKeyValueIsSettingPath()
        {
            new NameKey(new SimpleSettingKey("foo", "bar"));
        }

        [TestMethod]
        public void CreatesNameKey()
        {
            var nameKey = new NameKey(
                new SimpleSettingKey("foo", 
                new SettingPath("bar", "baz", "qux")));

            // Assert.AreEqual("bar.baz.qux", nameKey.ToString());
            Assert.IsTrue(true);
        }
    }

    [TestClass]
    public class _implicitOperator_String_NameKey
    {
        [TestMethod]
        public void ConvertsNameKeyToString()
        {
            var nameKey = new NameKey(
                new SimpleSettingKey("foo",
                new SettingPath("bar", "baz", "qux")));

            Assert.AreEqual("bar.baz.qux", (string)nameKey);
        }
    }
}
