using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass()]
    public class ObjectConverterExceptionTests
    {
        [TestMethod()]
        public void ctor_ObjectConverterException()
        {
            var ex = ExceptionAssert.Throws<ObjectConverterException>(() =>
            {
                throw new ObjectConverterException("abc", SettingInfo.From(() => ValueTypesTestConfig.BooleanField), null);

            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.AreEqual("abc", ex.Value);
            Assert.IsTrue(ex.Message.Contains("SettingPath = \"BooleanField\""));
            Assert.IsTrue(ex.Message.Contains("SettingType = \"Boolean\""));
        }
    }
}