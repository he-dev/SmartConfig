using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Validations;
using SmartConfig.DataAnnotations;

namespace SmartConfig.Core.Tests
{ // ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace

    [TestClass]
    public class GetCustomNameOrDefault
    {
        [TestMethod]
        public void GetCustomName()
        {
            var property = typeof(Foo).GetProperty(nameof(Foo.Bar), BindingFlags.Public | BindingFlags.Static);
            property.GetCustomNameOrDefault().Validate().IsEqual("Bar");
        }

        [TestMethod]
        public void GetDefaultName()
        {
            var property = typeof(Foo).GetProperty(nameof(Foo.Baz), BindingFlags.Public | BindingFlags.Static);
            property.GetCustomNameOrDefault().Validate().IsEqual("Qux");
        }
    }

    [TestClass]
    public class HasAttribute
    {
        [TestMethod]
        public void True()
        {
            typeof(Foo).HasAttribute<SmartConfigAttribute>().Validate().IsFalse();
        }

        [TestMethod]
        public void False()
        {
            typeof(Bar).HasAttribute<SmartConfigAttribute>().Validate().IsTrue();
        }

        private static class Foo { }

        [SmartConfig]
        private static class Bar { }
    }

    [TestClass]
    public class GetSettingPath
    {
        [TestMethod]
        public void GetPathWithoutSmartConfigName()
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
        [ExpectedException(typeof(SmartConfigAttributeNotFoundException))]
        public void ThrowsSmartConfigAttributeNotFoundException()
        {
            var barProperty =
                typeof(Bar.SubBar)
                    .GetProperty(nameof(Bar.SubBar.Baz), BindingFlags.Public | BindingFlags.Static)
                    .GetSettingPath();
        }        
    }

    internal static class Foo
    {
        public static string Bar { get; set; }

        [Rename("Qux")]
        public static string Baz { get; set; }
    }

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

    [TestClass]
    public class GetCustomNameOrDefault_ErrorHandling
    {
        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void NullType()
        {
            ((MemberInfo)null).GetCustomNameOrDefault();
        }
    }
}