using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class ObjectConverterNotFoundExceptionTests
    {
        [TestMethod]
        public void ctor_ObjectConverterNotFoundException()
        {
            var ex = ExceptionAssert.Throws<ObjectConverterNotFoundException>(() =>
            {
                throw new ObjectConverterNotFoundException(typeof(bool));
            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsTrue(ex.Message.Contains("ConverterType = \"Boolean\""));
            //Assert.IsTrue(ex.Message.Contains("ConfigType = \"ValueTypesTestConfig\""));
            //Assert.IsTrue(ex.Message.Contains("SettingPath = \"BooleanField\""));
        }
    }
}