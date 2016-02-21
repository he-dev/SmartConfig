using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Core.Tests;
using SmartConfig.Data;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Reflection.ReflectorTests
{
    [TestClass]
    public class IsStatic
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresType()
        {
            ((Type)null).IsStatic();
        }

        [TestMethod]
        public void ReturnsTrueIfTypeIsStatic()
        {
            Assert.IsTrue(typeof(StaticTestClass).IsStatic());
        }

        [TestMethod]
        public void ReturnsFalseIfTypeNotStatic()
        {
            Assert.IsFalse(typeof(NonStaticTestClass).IsStatic());
        }

        public static class StaticTestClass { }

        public class NonStaticTestClass { }
    }

    [TestClass]
    public class GetSettingNameOrMemberName
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresMemberInfo()
        {
            ((MemberInfo)null).GetSettingNameOrMemberName();
        }

        [TestMethod]
        public void GetsSettingName()
        {
            var testProperty = typeof(TestClass).GetProperty(nameof(TestClass.TestPropertyWithoutSettingNameAttribute), BindingFlags.Public | BindingFlags.Static);
            Assert.AreEqual(
                nameof(TestClass.TestPropertyWithoutSettingNameAttribute),
                testProperty.GetSettingNameOrMemberName());
        }

        [TestMethod]
        public void GetsMemberName()
        {
            var testProperty = typeof(TestClass).GetProperty(nameof(TestClass.TestPropertyWithSettingNameAttribute), BindingFlags.Public | BindingFlags.Static);
            Assert.AreEqual(
                "DifferentPropertyName",
                testProperty.GetSettingNameOrMemberName());
        }

        static class TestClass
        {
            public static string TestPropertyWithoutSettingNameAttribute { get; set; }

            [SettingName("DifferentPropertyName")]
            public static string TestPropertyWithSettingNameAttribute { get; set; }
        }
    }

    [TestClass]
    public class IsSmartConfig
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresType()
        {
            ((Type)null).IsSmartConfigType();
        }

        [TestMethod]
        public void ReturnTrueIfTypeHasSmartConfigAttribute()
        {
            Assert.IsFalse(typeof(ClassWithoutSmartConfigAttribute).IsSmartConfigType());
        }

        [TestMethod]
        public void ReturnTrueIfTypeDoesNotHaveSmartConfigAttribute()
        {
            Assert.IsTrue(typeof(ClassWithSmartConfigAttribute).IsSmartConfigType());
        }

        static class ClassWithoutSmartConfigAttribute
        {

        }

        [SmartConfig]
        static class ClassWithSmartConfigAttribute
        {

        }
    }

    [TestClass]
    public class GetSettingPath
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresPropertyInfo()
        {
            ((PropertyInfo)null).GetSettingPath();
        }

        [TestMethod]
        public void GetsPathWithoutSmartConfigType()
        {
            var barProperty = typeof(Foo.SubFoo.SubSubFoo.SubSubSubFoo).GetProperty(nameof(Foo.SubFoo.SubSubFoo.SubSubSubFoo.Bar), BindingFlags.Public | BindingFlags.Static);
            CollectionAssert.AreEqual(
                new string[]
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
        static class Foo
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

        static class Bar
        {
            public static class SubBar
            {
                public static string Baz { get; set; }
            }
        }
    }

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

    [TestClass]
    public class GetSettingKeyNames
    {
        [TestMethod]
        public void GetsMainKeyName()
        {
            CollectionAssert.AreEqual(
                new[] { "Name" },
                Reflector.GetSettingKeyNames<BasicSetting>().ToList()
            );
        }

        [TestMethod]
        public void GetsCustomKeyNames()
        {
            CollectionAssert.AreEqual(
                new[] { "Name", "Environment", "Version" },
                Reflector.GetSettingKeyNames<CustomTestSetting>().ToList()
            );
        }
    }
}