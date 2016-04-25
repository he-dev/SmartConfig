using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Filters;

namespace SmartConfig.Core.Tests.Filters
{
    [TestClass]
    public class WildcardComparerTests
    {
        [TestMethod]
        public void SortStrings()
        {
            var result = new[] { "c", "*", "b", "a" }.OrderBy(x => x, new WildcardComparer()).ToList();
            CollectionAssert.AreEqual(new[] { "c", "b", "a", "*" }, result);
        }
    }
}
