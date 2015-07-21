using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace SmartConfig.Tests
{
    [TestClass]
    public class SemanticVersionTests
    {
        [TestMethod]
        public void TestParse()
        {
            var semVer = SemanticVersion.Parse("1.2.3");
            Assert.AreEqual(1, semVer.Major);
            Assert.AreEqual(2, semVer.Minor);
            Assert.AreEqual(3, semVer.Patch);

            semVer = SemanticVersion.Parse("v1.2.3-beta");
            Assert.AreEqual(1, semVer.Major);
            Assert.AreEqual(2, semVer.Minor);
            Assert.AreEqual(3, semVer.Patch);
        }

        [TestMethod]
        public void TestEquality()
        {
            Assert.IsTrue(SemanticVersion.Parse("1.0.0") == SemanticVersion.Parse("1.0.0"));
            Assert.IsFalse(SemanticVersion.Parse("1.1.0") == null);
            var semVer = SemanticVersion.Parse("1.1.1");
            Assert.IsTrue(semVer == semVer);
        }

        [TestMethod]
        public void TestCompareToLessThenZero()
        {
            Assert.IsTrue(SemanticVersion.Parse("1.0.0") < SemanticVersion.Parse("2.0.0"));
            Assert.IsTrue(SemanticVersion.Parse("1.1.0") < SemanticVersion.Parse("1.2.0"));
            Assert.IsTrue(SemanticVersion.Parse("1.1.1") < SemanticVersion.Parse("1.1.2"));
        }

        [TestMethod]
        public void TestCompareToZero()
        {
            Assert.IsTrue(SemanticVersion.Parse("1.0.0") == SemanticVersion.Parse("1.0.0"));
            Assert.IsTrue(SemanticVersion.Parse("1.1.0") == SemanticVersion.Parse("1.1.0"));
            Assert.IsTrue(SemanticVersion.Parse("1.1.1") == SemanticVersion.Parse("1.1.1"));
        }

        [TestMethod]
        public void TestCompareToGreaterThenZero()
        {
            Assert.IsTrue(SemanticVersion.Parse("2.0.0") > SemanticVersion.Parse("1.0.0"));
            Assert.IsTrue(SemanticVersion.Parse("2.2.0") > SemanticVersion.Parse("2.1.0"));
            Assert.IsTrue(SemanticVersion.Parse("2.2.2") > SemanticVersion.Parse("2.2.1"));
        }

        [TestMethod]
        public void TestLessThenOrEqual()
        {
            Assert.IsTrue(SemanticVersion.Parse("1.0.0") <= SemanticVersion.Parse("2.0.0"));
            Assert.IsTrue(SemanticVersion.Parse("1.12.123") <= SemanticVersion.Parse("1.12.123"));
        }

        [TestMethod]
        public void TestGreaterThenOrEqual()
        {
            Assert.IsTrue(SemanticVersion.Parse("2.0.1") >= SemanticVersion.Parse("2.0.0"));
            Assert.IsTrue(SemanticVersion.Parse("2.23.234") >= SemanticVersion.Parse("2.23.234"));
        }

        [TestMethod]
        public void TestSort()
        {
            var semVers = new List<SemanticVersion>
            {
                SemanticVersion.Parse("2.0.0"),
                SemanticVersion.Parse("3.2.0"),
                null,
                SemanticVersion.Parse("1.0.3"),
            };

            var sortedSemVers = semVers.OrderBy(sv => sv).ToList();
            Assert.IsNull(sortedSemVers[0]);
            Assert.AreEqual("1.0.3", sortedSemVers[1].ToString());
            Assert.AreEqual("2.0.0", sortedSemVers[2].ToString());
            Assert.AreEqual("3.2.0", sortedSemVers[3].ToString());
        }
    }
}
