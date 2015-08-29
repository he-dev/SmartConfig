using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities;

namespace SmartConfig.Tests
{
    [TestClass()]
    public class DataSourceExceptionTests
    {
        [TestMethod()]
        public void ctor_DataSourceException()
        {
            var ex = ExceptionAssert.Throws<DataSourceException>(() =>
            {
                throw new DataSourceException(new AppConfigSource(), SettingInfo.From(() => ValueTypesTestConfig.BooleanField), null);

            }, Assert.Fail);
            Assert.IsNotNull(ex);
            Assert.IsTrue(ex.Message.Contains("AppConfigSource"));
            Assert.IsTrue(ex.Message.Contains("BooleanField"));
        }
    }
}