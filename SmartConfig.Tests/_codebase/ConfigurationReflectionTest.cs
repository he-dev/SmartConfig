using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Core.Tests.ConfigurationReflectionTestData;
using SmartConfig.Data.Annotations;

namespace SmartConfig.Core.Tests
{
    [TestClass]
    public class ConfigurationReflectionTest
    {
        [TestMethod]
        public void HasAttribute_True()
        {
            typeof(MissingAttributeConfig).HasAttribute<SmartConfigAttribute>().Verify().IsFalse();
        }

        [TestMethod]
        public void HasAttribute_False()
        {
            typeof(EmpyConfig).HasAttribute<SmartConfigAttribute>().Verify().IsTrue();
        }

        [TestMethod]
        public void GetPath_NoConfigName_PathWithoutConfigName()
        {
            var barProperty =
                typeof(Baz.SubBaz.SubSubBaz.SubSubSubBaz)
                .GetProperty(nameof(Baz.SubBaz.SubSubBaz.SubSubSubBaz.Bar), BindingFlags.Public | BindingFlags.Static);

            CollectionAssert.AreEqual(
                new[]
                {
                    nameof(Baz.SubBaz),
                    nameof(Baz.SubBaz.SubSubBaz),
                    nameof(Baz.SubBaz.SubSubBaz.SubSubSubBaz),
                    nameof(Baz.SubBaz.SubSubBaz.SubSubSubBaz.Bar),
                },
                barProperty.GetPath().ToList());
        }

        [TestMethod]
        public void GetPath_ConfigWithName_PathWithConfigName()
        {
            var barProperty =
                typeof(Foo2)
                .GetProperty(nameof(Foo2.Baz), BindingFlags.Public | BindingFlags.Static);

            CollectionAssert.AreEqual(
                new[] { "Corge", "Qux" },
                barProperty.GetPath().ToList()
            );
        }
    }
}