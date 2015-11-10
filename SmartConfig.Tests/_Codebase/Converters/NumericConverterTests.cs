using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class NumericConverterTests
    {
        [TestMethod]
        public void DeserializeObject()
        {
            var numericConverter = new NumericConverter();
            var ci = CultureInfo.InvariantCulture;

            Assert.AreEqual(byte.MaxValue, numericConverter.DeserializeObject(byte.MaxValue.ToString(), typeof(byte), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(char.MaxValue, numericConverter.DeserializeObject(char.MaxValue.ToString(), typeof(char), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(short.MaxValue, numericConverter.DeserializeObject(short.MaxValue.ToString(), typeof(short), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(ushort.MaxValue, numericConverter.DeserializeObject(ushort.MaxValue.ToString(), typeof(ushort), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(int.MaxValue, numericConverter.DeserializeObject(int.MaxValue.ToString(), typeof(int), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(uint.MaxValue, numericConverter.DeserializeObject(uint.MaxValue.ToString(), typeof(uint), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(long.MaxValue, numericConverter.DeserializeObject(long.MaxValue.ToString(), typeof(long), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(ulong.MaxValue, numericConverter.DeserializeObject(ulong.MaxValue.ToString(), typeof(ulong), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(float.MaxValue, numericConverter.DeserializeObject(float.MaxValue.ToString("R", ci), typeof(float), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(double.MaxValue, numericConverter.DeserializeObject(double.MaxValue.ToString("R", ci), typeof(double), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(decimal.MaxValue, numericConverter.DeserializeObject(decimal.MaxValue.ToString(ci), typeof(decimal), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void SerializeObject()
        {
            //var valueTypeConverter = new ValueTypeConverter();
            //Assert.AreEqual("123", valueTypeConverter.SerializeObject(123, typeof(int), Enumerable.Empty<ConstraintAttribute>()));
            //Assert.AreEqual("123", valueTypeConverter.SerializeObject((Int32?)123, typeof(int), Enumerable.Empty<ConstraintAttribute>()));
            //Assert.IsNull(valueTypeConverter.SerializeObject(null, typeof(int?), Enumerable.Empty<ConstraintAttribute>()));
        }
    }
}
