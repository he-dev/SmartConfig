using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Collections
{
    [TestClass()]
    public class ObjectConverterDictionaryTests
    {
        [TestMethod()]
        public void Add_Converter()
        {
            var valueTypeConverter = new ValueTypeConverter();
            var objectConverterDictionary = new ObjectConverterDictionary
            {
                valueTypeConverter
            };

            Assert.IsNotNull(objectConverterDictionary[typeof(int)]);
            Assert.IsNull(objectConverterDictionary[typeof(DateTime)]);
        }
    }
}