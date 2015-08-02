using System;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class ColorConverterTests
    {
        [TestMethod]
        public void DeserializeObject_ThreeColors()
        {
            var converter = new SmartConfig.Converters.ColorConverter();
            Assert.AreEqual(Color.FromArgb(255, 0, 0), converter.DeserializeObject("Red", typeof(Color), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(Color.FromArgb(1, 2, 3), converter.DeserializeObject("1,2,3", typeof(Color), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(Color.FromArgb(255, 1, 2), converter.DeserializeObject("#FF0102", typeof(Color), Enumerable.Empty<ConstraintAttribute>()));
        }        
    }
}
