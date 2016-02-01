using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartConfig.Reflection;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Reflection.SettingInfoTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void RequiresPropertyInfo()
        {
        }

        [TestMethod]
        public void RequiresConfigurationInfo()
        {
        }
    }

    [TestClass]
    public class ConverterType
    {
        [TestMethod]
        public void GetsEnumType()
        {
            var property = typeof(Foo).GetProperty(nameof(Foo.Baz), BindingFlags.Public | BindingFlags.Static);
            var settingInfo = new SettingInfo(property, new ConfigurationInfo(typeof(Foo)));
            Assert.IsTrue(settingInfo.ConverterType == typeof(Enum));
        }

        [TestMethod]
        public void GetsPropertyType()
        {
            var property = typeof(Foo).GetProperty(nameof(Foo.Bar), BindingFlags.Public | BindingFlags.Static);
            var settingInfo = new SettingInfo(property, new ConfigurationInfo(typeof(Foo)));
            Assert.IsTrue(settingInfo.ConverterType == typeof(int));
        }

        [TestMethod]
        public void GetsCustomType()
        {
            var property = typeof(Foo).GetProperty(nameof(Foo.Qux), BindingFlags.Public | BindingFlags.Static);
            var settingInfo = new SettingInfo(property, new ConfigurationInfo(typeof(Foo)));
            Assert.IsTrue(settingInfo.ConverterType == typeof(JsonConverter));
        }

        // --- test data

        [SmartConfig]
        static class Foo
        {
            static public Baz Baz { get; set; }
            static public int Bar { get; set; }

            [ObjectConverter(typeof(JsonConverter))]
            static public double[] Qux { get; set; }
        }

        enum Baz { }
    }

    [TestClass]
    public class SettingPath
    {
        [TestMethod]
        public void GetsWithoutConfigurationName()
        {
            var property = typeof(Foo).GetProperty(nameof(Foo.Bar), BindingFlags.Public | BindingFlags.Static);
            var settingInfo = new SettingInfo(property, new ConfigurationInfo(typeof(Foo)));
            Assert.IsTrue(settingInfo.SettingPath == new SmartConfig.Paths.SettingPath(null, "Bar"));
        }

        [TestMethod]
        public void GetsWithConfigurationName()
        {
            var property = typeof(Baz).GetProperty(nameof(Baz.Qux), BindingFlags.Public | BindingFlags.Static);
            var settingInfo = new SettingInfo(property, new ConfigurationInfo(typeof(Baz)));
            Assert.IsTrue(settingInfo.SettingPath == new SmartConfig.Paths.SettingPath("Quux", "Qux"));
        }

        [SmartConfig]
        static class Foo
        {
            static public string Bar { get; set; }
        }

        [SmartConfig]
        [SettingName("Quux")]
        static class Baz
        {
            static public string Qux { get; set; }
        }
    }

    [TestClass]
    public class IsOptional
    {
        [TestMethod]
        public void GetsFalse()
        {
            var property = typeof(Foo).GetProperty(nameof(Foo.Bar), BindingFlags.Public | BindingFlags.Static);
            var settingInfo = new SettingInfo(property, new ConfigurationInfo(typeof(Foo)));
            Assert.IsFalse(settingInfo.IsOptional);
        }

        [TestMethod]
        public void GetsTrue()
        {
            var property = typeof(Foo).GetProperty(nameof(Foo.Baz), BindingFlags.Public | BindingFlags.Static);
            var settingInfo = new SettingInfo(property, new ConfigurationInfo(typeof(Foo)));
            Assert.IsTrue(settingInfo.IsOptional);
        }

        [SmartConfig]
        static class Foo
        {
            static public string Bar { get; set; }

            [Optional]
            static public string Baz{ get; set; }
        }
    }
}
