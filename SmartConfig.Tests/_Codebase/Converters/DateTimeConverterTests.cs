using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class DateTimeConverterTests
    {
        [TestMethod]
        public void DeserializeObject_CanDeserializeDateTime()
        {
            var converter = new DateTimeConverter();
            Assert.AreEqual(new DateTime(2015, 12, 7, 19, 27, 33), converter.DeserializeObject("12/07/2015 19:27:33", typeof(DateTime), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void DeserializeObject_CanDeserializeDateTimeWithCustomFormat()
        {
            var converter = new DateTimeConverter();
            Assert.AreEqual(new DateTime(2015, 12, 7, 19, 27, 33), converter.DeserializeObject("2015-12-07 19:27:33", typeof(DateTime), new[] { new DateTimeFormatAttribute("yyyy-MM-dd HH:mm:ss"), }));
        }

        [TestMethod]
        public void DeserializeObject_ThrowsExceptionForInvalidDateTime()
        {
            var converter = new DateTimeConverter();
            ExceptionAssert.Throws<FormatException>(() =>
            {
                converter.DeserializeObject("abc", typeof(DateTime), Enumerable.Empty<ConstraintAttribute>());
            }, ex => { }, Assert.Fail);
        }

        [TestMethod]
        public void SerializeObject_CanSerializeDateTime()
        {
            var converter = new DateTimeConverter();
            Assert.AreEqual("12/07/2015 19:27:33", converter.SerializeObject(new DateTime(2015, 12, 7, 19, 27, 33), typeof(DateTime), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void SerializeObject_ThrowsExceptionForInvalidDateTime()
        {
            var converter = new DateTimeConverter();
            ExceptionAssert.Throws<TargetException>(() =>
            {
                converter.SerializeObject("abc", typeof(DateTime), Enumerable.Empty<ConstraintAttribute>());
            }, ex => { }, Assert.Fail);
        }
    }
}
