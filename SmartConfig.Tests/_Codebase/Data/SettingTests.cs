using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartConfig.Filters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Core.Tests.Data.SettingTests
{
    [TestClass]
    public class indexerTests
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
            ExceptionAssert.Throws<InvalidPropertyNameException>(() =>
            {
                var setting = new BasicSetting
                {
                    Name = "n",
                    Value = "v"
                };
                var value = setting["b"];
            }, ex =>
            {
                Assert.AreEqual("b", ex.PropertyName);
                Assert.AreEqual(typeof(BasicSetting).Name, ex.SettingType);
            },
            Assert.Fail);
        }

        public class TestSetting : BasicSetting
        {
            [SettingFilter(typeof(StringFilter))]
            public string Foo { get; set; }
        }
    }
}
