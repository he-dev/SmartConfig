using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    [TestClass]
    public class WeakSettingPathComparerTest
    {
        [TestMethod]
        public void Equals_WeakPathAndStrongPath_True()
        {
            SettingPath.Parse("Foo[0]").IsLike(SettingPath.Parse("Foo")).Verify().IsTrue();
        }

        [TestMethod]
        public void Equals_StrongPathAndStrongPath_True()
        {
            SettingPath.Parse("Foo[0]").IsLike(SettingPath.Parse("Foo[2]")).Verify().IsTrue();
        }

        [TestMethod]
        public void Equals_DifferentStrongPaths_False()
        {
            SettingPath.Parse("Foo[0]").IsLike(SettingPath.Parse("Bar[2]")).Verify().IsFalse();
        }
    }
}
