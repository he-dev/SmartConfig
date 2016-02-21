using System;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities;
using ColorConverter = SmartConfig.Converters.ColorConverter;

namespace SmartConfig.Core.Tests.Converters.ColorConverterTests
{
    [TestClass]
    public class DeserializeTests
    {
        [TestMethod]
        public void DeserializesName()
        {
            var converter = new ColorConverter();
            Assert.AreEqual(Color.FromArgb(255, 0, 0),
                converter.DeserializeObject("Red", typeof(Color), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void DeserializesDec()
        {
            var converter = new ColorConverter();
            Assert.AreEqual(Color.FromArgb(1, 2, 3),
                converter.DeserializeObject("1,2,3", typeof(Color), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void DeserializesHex()
        {
            var converter = new ColorConverter();
            Assert.AreEqual(Color.FromArgb(255, 1, 2),
                converter.DeserializeObject("#FF0102", typeof(Color), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void ThrowsInvalidValueException()
        {
            var converter = new ColorConverter();
            converter.DeserializeObject("foo", typeof(Color32), Enumerable.Empty<Attribute>());
        }
    }

    [TestClass]
    public class SerializeTests
    {
        [TestMethod]
        public void SerializesColor()
        {
            var color = Color.FromArgb(255, 1, 2);
            var converter = new ColorConverter();
            var value = converter.SerializeObject(color, typeof(string), Enumerable.Empty<ConstraintAttribute>());
            Assert.AreEqual("#FF0102", value);
        }

        [TestMethod]
        [ExpectedException(typeof(UnsupportedTypeException))]
        public void ThrowsSerializationException()
        {
            var converter = new ColorConverter();
            converter.SerializeObject(1, typeof(string), Enumerable.Empty<Attribute>());
        }
    }
}
