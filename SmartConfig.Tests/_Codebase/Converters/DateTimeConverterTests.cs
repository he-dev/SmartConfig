using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartUtilities.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Converters.DateTimeConverterTests
{
    [TestClass]
    public class DeserializeTests
    {
        [TestMethod]
        public void DeserializesDefaultFormats()
        {
            var converter = new DateTimeConverter();
            Assert.AreEqual(
                new DateTime(2015, 7, 12, 19, 27, 33),
                converter.DeserializeObject("2015-07-12 19:27:33", typeof(DateTime), Enumerable.Empty<ConstraintAttribute>()));

            Assert.AreEqual(
                new DateTime(2015, 7, 9, 3, 25, 1),
                converter.DeserializeObject("2015-7-9 3:25:1", typeof(DateTime), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void DeserializesWithCustomFormat()
        {
            var converter = new DateTimeConverter();
            Assert.AreEqual(
                new DateTime(2015, 12, 7, 19, 27, 33),
                converter.DeserializeObject("2015.12.07 19:27:33", typeof(DateTime), new[] { new DateTimeFormatAttribute("yyyy.MM.dd HH:mm:ss") }));
        }

        [TestMethod]
        public void ThrowsInvalidValueException()
        {
            var converter = new DateTimeConverter();

            ExceptionAssert.Throws<InvalidValueException>(() =>
            {
                converter.DeserializeObject("abc", typeof(DateTime), Enumerable.Empty<ConstraintAttribute>());
            }, ex => { }, Assert.Fail);

            ExceptionAssert.Throws<InvalidValueException>(() =>
            {
                converter.DeserializeObject("abc", typeof(DateTime), new[] { new DateTimeFormatAttribute("yyyy.MM.dd HH:mm:ss") });
            }, ex => { }, Assert.Fail);
        }
    }

    [TestClass]
    public class SerializeTests
    {
        [TestMethod]
        public void SerializeObject_CanSerializeDateTime()
        {
            var converter = new DateTimeConverter();
            Assert.AreEqual("2015-12-07 19:27:33", converter.SerializeObject(new DateTime(2015, 12, 7, 19, 27, 33), typeof(string), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void SerializeObject_ThrowsExceptionForInvalidDateTime()
        {
            var converter = new DateTimeConverter();
            ExceptionAssert.Throws<UnsupportedTypeException>(() =>
            {
                converter.SerializeObject("abc", typeof(DateTime), Enumerable.Empty<ConstraintAttribute>());
            }, ex => { }, Assert.Fail);
        }
    }
}
