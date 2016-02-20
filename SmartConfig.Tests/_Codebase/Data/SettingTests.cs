using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class SettingTests_indexer
    {
        [TestMethod]
        public void GetsCustomPropertyByName()
        {
            var setting = new TestSetting
            {
                Name = "n",
                Value = "v",
                Foo = "f"
            };

            Assert.AreEqual("f", setting[nameof(TestSetting.Foo)]);
        }

        [TestMethod]
        public void ThrowsInvalidPropertyNameException()
        {
            ExceptionAssert.Throws<InvalidPropertyNameException>(
                () =>
                {
                    var setting = new TestSetting
                    {
                        Name = "n",
                        Value = "v",
                        Foo = "f"
                    };
                    var value = setting["b"];
                }, ex =>
                {
                    Assert.AreEqual("b", ex.PropertyName);
                    Assert.AreEqual(typeof(TestSetting).FullName, ex.TargetType);
                },
                Assert.Fail);
        }

        public class TestSetting : BasicSetting
        {
            public string Foo { get; set; }
        }
    }
}
