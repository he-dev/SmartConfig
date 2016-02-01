using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Converters;
using SmartUtilities.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Collections.ObjectConverterCollectionTests
{
    [TestClass]
    public class _CollectionInitializer
    {
        [TestMethod]
        public void AddsConverters()
        {
            var converters = new ObjectConverterCollection
            {
                new Foo()
            };
            Assert.AreEqual(2, converters.TypeCount);
            Assert.IsInstanceOfType(converters[typeof (int)], typeof (Foo));
            Assert.IsInstanceOfType(converters[typeof (float)], typeof (Foo));
        }

        class Foo : ObjectConverter
        {
            public Foo() : base(new[] { typeof(int), typeof(float) }) { }

            public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
            {
                throw new NotImplementedException();
            }

            public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
            {
                throw new NotImplementedException();
            }
        }
    }
}