using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartUtilities;

namespace SmartConfig.Tests
{
    [TestClass()]
    public class UnsupportedTypeExceptionTests
    {
        [TestMethod()]
        public void ctor_UnsupportedTypeException()
        {
            var ex = ExceptionAssert.Throws<UnsupportedTypeException>(() =>
            {
                throw new UnsupportedTypeException(typeof(ValueTypeConverter), typeof(DateTimeConverter));

            }, Assert.Fail);

            Assert.IsNotNull(ex);
            Assert.IsTrue(ex.Message.Contains("Converter = \"ValueTypeConverter\""));
            Assert.IsTrue(ex.Message.Contains("SettingType = \"DateTimeConverter\""));
        }
    }
}