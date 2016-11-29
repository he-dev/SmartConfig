using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data.Annotations;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.Unit
{
    using ConfigurationReflectionTestConfigs;
    using Reusable.Fuse;
    using Reusable.Fuse.Testing;

    [TestClass]
    public class ConfigurationReflectionTest
    {
        [TestMethod]
        public void GetSettingProperties_FromNestedWithIgnore()
        {
            var settings = typeof(Foo).GetSettingProperties();
            settings.Count.Verify().IsEqual(3);
        }

        [TestMethod]
        public void GetCustomNameOrDefault_Custom()
        {
            var property = typeof(Foo2).GetProperty(nameof(Foo2.Bar), BindingFlags.Public | BindingFlags.Static);
            property.GetCustomNameOrDefault().Verify().IsEqual("Bar");
        }

        [TestMethod]
        public void GetCustomNameOrDefault_Default()
        {
            var property = typeof(Foo2).GetProperty(nameof(Foo2.Baz), BindingFlags.Public | BindingFlags.Static);
            property.GetCustomNameOrDefault().Verify().IsEqual("Qux");
        }

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
        public void GetSettingPath_PathWithoutConfigName()
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
                barProperty.GetSettingPath().ToList());
        }

        [TestMethod]
        public void GetSettingPath_Throws_ArgumentException()
        {
            new Action(() =>
            {
                typeof(Bar.SubBar)
                    .GetProperty(nameof(Bar.SubBar.Baz), BindingFlags.Public | BindingFlags.Static)
                    .GetSettingPath();
            })
            .Verify()
            .Throws<ArgumentException>();
        }


    }
}

namespace SmartConfig.Core.Tests.Unit.ConfigurationReflectionTestConfigs
{
    [SmartConfig]
    internal static class Foo
    {
        public static string Bar1 { get; set; }
        public static string Bar2 { get; set; }

        public static class SubFoo
        {
            public static string Baz1 { get; set; }

            [Reusable.Data.Annotations.Ignore]
            public static string Baz2 { get; set; }
        }

        [Reusable.Data.Annotations.Ignore]
        public static class SubBaz
        {
            public static string Quux { get; set; }

            public static class SubSubBaz
            {
                public static string SubQuux { get; set; }
            }
        }
    }

    internal static class Foo2
    {
        public static string Bar { get; set; }

        [Rename("Qux")]
        public static string Baz { get; set; }
    }

    internal static class MissingAttributeConfig { }

    [SmartConfig]
    internal static class EmpyConfig { }

    [SmartConfig]
    internal static class Baz
    {
        public static class SubBaz
        {
            public static class SubSubBaz
            {
                public static class SubSubSubBaz
                {
                    public static string Bar { get; set; }
                }
            }
        }
    }

    internal static class Bar
    {
        public static class SubBar
        {
            public static string Baz { get; set; }
        }
    }
}