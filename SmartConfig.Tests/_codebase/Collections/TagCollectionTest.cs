using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;

namespace SmartConfig.Core.Tests.Collections
{
    [TestClass]
    public class TagCollectionTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_ReservedName_ArgumentException()
        {
            var tc = new TagCollection
            {
                { "Name", "T1" }
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_ReservedValue_ArgumentException()
        {
            var tc = new TagCollection
            {
                { "Value", "T1" }
            };
        }
    }
}
