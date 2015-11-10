using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Converters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests.Collections
{
    [TestClass]
    public class ObjectConverterCollectionTests
    {
        [TestMethod]
        public void CollectionInitializer_CanAddConverters()
        {
            var converters = new ObjectConverterCollection
            {
                new NumericConverter(),
                new BooleanConverter()
            };
            Assert.AreEqual(13, converters.Count());
        }

        [TestMethod]
        public void CollectionInitializer_FailsToAddDuplicateConverters()
        {
            ExceptionAssert.Throws<DuplicateTypeConverterException>(() =>
            {
                var converters = new ObjectConverterCollection
                {
                    new NumericConverter(),
                    new NumericConverter(),
                };
            }, ex => { }, Assert.Fail);
        }
    }
}