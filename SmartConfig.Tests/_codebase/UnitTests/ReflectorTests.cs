using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Core.Tests;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.ValidationExtensions;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.ReflectorTests
{
    //[TestClass]
    //public class IsStatic
    //{
    //    [TestMethod]
    //    [ExpectedException(typeof(ArgumentNullException))]
    //    public void RequiresType()
    //    {
    //        ((Type)null).IsStatic();
    //    }

    //    [TestMethod]
    //    public void ReturnsTrueIfTypeIsStatic()
    //    {
    //        Assert.IsTrue(typeof(StaticTestClass).IsStatic());
    //    }

    //    [TestMethod]
    //    public void ReturnsFalseIfTypeNotStatic()
    //    {
    //        Assert.IsFalse(typeof(NonStaticTestClass).IsStatic());
    //    }

    //    public static class StaticTestClass { }

    //    public class NonStaticTestClass { }
    //}


    namespace UseCases
    {
        [TestClass]
        public class GetCustomNameOrDefault
        {
            [TestMethod]
            public void GetCustomName()
            {
                var property = typeof (Foo).GetProperty(nameof(Foo.Bar), BindingFlags.Public | BindingFlags.Static);
                property.GetCustomNameOrDefault().Validate().IsEqual("Bar");
            }

            [TestMethod]
            public void GetDefaultName()
            {
                var property = typeof (Foo).GetProperty(nameof(Foo.Baz), BindingFlags.Public | BindingFlags.Static);
                property.GetCustomNameOrDefault().Validate().IsEqual("Qux");
            }

            private static class Foo
            {
                public static string Bar { get; set; }

                [Rename("Qux")]
                public static string Baz { get; set; }
            }
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

    [TestClass]
    public class HasAttribute_UseCases
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
    public class GetSettingPath_UseCases
    {
        [TestMethod]
        public void GetPathWithoutSmartConfigName()
        {
            var barProperty = 
                typeof(Foo.SubFoo.SubSubFoo.SubSubSubFoo)
                .GetProperty(nameof(Foo.SubFoo.SubSubFoo.SubSubSubFoo.Bar), BindingFlags.Public | BindingFlags.Static);
            CollectionAssert.AreEqual(
                new []
                {
                    nameof(Foo.SubFoo),
                    nameof(Foo.SubFoo.SubSubFoo),
                    nameof(Foo.SubFoo.SubSubFoo.SubSubSubFoo),
                    nameof(Foo.SubFoo.SubSubFoo.SubSubSubFoo.Bar),
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

        [SmartConfig]
        private static class Foo
        {
            public static class SubFoo
            {
                public static class SubSubFoo
                {
                    public static class SubSubSubFoo
                    {
                        public static string Bar { get; set; }
                    }
                }
            }
        }

        private static class Bar
        {
            public static class SubBar
            {
                public static string Baz { get; set; }
            }
        }
    }

    //[TestClass]
    //public class GetTypesTests
    //{
    //    //[TestMethod]
    //    //[ExpectedException(typeof(TypeNotStaticException))]
    //    //public void RequiresTypeIsStatic()
    //    //{
    //    //    SettingCollection.GetSettingGroups(typeof(Bar), new List<Type>());
    //    //}

    //    [TestMethod]
    //    public void GetsTypes()
    //    {
    //        var types = typeof(Foo).GetTypes().ToList();
    //        Assert.AreEqual(6, types.Count);

    //        // make sure the types that shouldn't be there aren't there
    //        Assert.IsNull(types.SingleOrDefault(t => t == typeof(Foo.SubFoo2.Baz)));
    //    }

    //    [SmartConfig]
    //    static class Foo
    //    {
    //        //[SmartConfigProperties]
    //        //public static class Bar { }

    //        public static class SubFoo1 { }

    //        public static class SubFoo2
    //        {
    //            public static class SubSubFoo1
    //            {
    //                public static class SubSubSubFoo1 { }
    //            }

    //            public static class SubSubFoo2 { }

    //            [SmartUtilities.ObjectConverters.DataAnnotations.Ignore]
    //            public static class Baz { }
    //        }
    //    }

    //    [SmartConfig]
    //    private static class Bar
    //    {
    //        public static class SubBar1
    //        {
    //        }

    //        public class SubBar2
    //        {
    //        }
    //    }
    //}

    //[TestClass]
    //public class GetConfigurationTypes
    //{
    //    [TestMethod]
    //    [ExpectedException(typeof(ArgumentNullException))]
    //    public void RequiresType()
    //    {
    //        var types = ((Type)null).GetConfigurationTypes();
    //    }

    //    [TestMethod]
    //    [ExpectedException(typeof(TypeNotStaticException))]
    //    public void RequiresTypeIsStatic()
    //    {
    //        var types = typeof(Bar).GetConfigurationTypes();
    //    }

    //    [TestMethod]
    //    public void GetsOnlyRelevantTypes()
    //    {
    //        var types = typeof(Foo).GetConfigurationTypes().ToList();
    //        Assert.AreEqual(6, types.Count);

    //        // make sure the types that shouldn't be there aren't there
    //        //Assert.IsNull(types.SingleOrDefault(t => t == typeof(Foo.Bar)));
    //        Assert.IsNull(types.SingleOrDefault(t => t == typeof(Foo.SubFoo2.Baz)));
    //    }

    //    [SmartConfig]
    //    static class Foo
    //    {
    //        //[SmartConfigProperties]
    //        //public static class Bar { }

    //        public static class SubFoo1 { }

    //        public static class SubFoo2
    //        {
    //            public static class SubSubFoo1
    //            {
    //                public static class SubSubSubFoo1 { }
    //            }

    //            public static class SubSubFoo2 { }

    //            [Ignore]
    //            public static class Baz { }
    //        }
    //    }

    //    [SmartConfig]
    //    private static class Bar
    //    {
    //        public static class SubBar1
    //        {
    //        }

    //        public class SubBar2
    //        {
    //        }
    //    }
    //}

    //[TestClass]
    //public class GetSettingInfos
    //{
    //    [TestMethod]
    //    [ExpectedException(typeof(ArgumentNullException))]
    //    public void RequiresConfigurationInfo()
    //    {
    //        ((Configuration)null).GetSettings();
    //    }

    //    [TestMethod]
    //    public void GetsRelevantSettingInfos()
    //    {
    //        var configurationInfo = new Configuration(typeof(Foo));
    //        var settingInfos = configurationInfo.GetSettings().ToList();
    //        Assert.AreEqual(3, settingInfos.Count);
    //    }

    //    [SmartConfig]
    //    static class Foo
    //    {
    //        static public string Bar1 { get; set; }
    //        static public string Bar2 { get; set; }

    //        public static class SubFoo
    //        {
    //            static public string Baz1 { get; set; }

    //            [Ignore]
    //            static public string Baz2 { get; set; }
    //        }
    //    }
    //}

    //[TestClass]
    //public class GetSettingKeyNames
    //{
    //    [TestMethod]
    //    public void GetsMainKeyName()
    //    {
    //        CollectionAssert.AreEqual(
    //            new[] { "Main" },
    //            Reflector.GetSettingKeyNames<BasicSetting>().ToList()
    //        );
    //    }

    //    [TestMethod]
    //    public void GetsCustomKeyNames()
    //    {
    //        CollectionAssert.AreEqual(
    //            new[] { "Main", "Environment", "Version" },
    //            Reflector.GetSettingKeyNames<CustomTestSetting>().ToList()
    //        );
    //    }
    //}
}