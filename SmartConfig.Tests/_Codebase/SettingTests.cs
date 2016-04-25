using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.DataAnnotations;
using SmartUtilities.ObjectConverters;
using SmartUtilities.ObjectConverters.DataAnnotations;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.SettingTests
{
    [TestClass]
    public class NormalizedTypeTests
    {
        [TestMethod]
        public void GetsEnumType()
        {
            var configuration = Configuration.Load(typeof(Foo));
            var setting = configuration.Settings.Single(s => s.Path == "Baz");
            Assert.IsTrue(setting.NormalizedType == typeof(Enum));
        }

        [TestMethod]
        public void GetsPropertyType()
        {
            var configuration = Configuration.Load(typeof(Foo));
            var setting = configuration.Settings.Single(s => s.Path == "Bar");
            Assert.IsTrue(setting.NormalizedType == typeof(int));
        }        

        // --- test data

        [SmartConfig]
        private static class Foo
        {
            public static Baz Baz { get; set; }
            public static int Bar { get; set; }
            public static double[] Qux { get; set; }
        }

        private enum Baz { }
    }

    [TestClass]
    public class SettingPathTests
    {
        [TestMethod]
        public void GetsWithoutConfigurationName()
        {
            var configuration = Configuration.Load(typeof(Foo));
            var setting = configuration.Settings.SingleOrDefault(s => s.Path == "Bar");
            Assert.IsNotNull(setting, "Could not find setting by path.");
        }

        [TestMethod]
        public void GetsWithConfigurationName()
        {
            var configuration = Configuration.Load(typeof(Baz));
            var setting = configuration.Settings.SingleOrDefault(s => s.Path == "Quux.Qux");
            Assert.IsNotNull(setting, "Could not find setting by path.");
        }

        [SmartConfig]
        private static class Foo
        {
            public static string Bar { get; set; }
        }

        [SmartConfig]
        [CustomName("Quux")]
        private static class Baz
        {
            public static string Qux { get; set; }
        }
    }

    [TestClass]
    public class IsOptional
    {
        [TestMethod]
        public void GetsFalse()
        {
            var configuration = Configuration.Load(typeof(Foo));
            var setting = configuration.Settings.Single(s => s.Path == "Bar");
            Assert.IsFalse(setting.IsOptional);
        }

        [TestMethod]
        public void GetsTrue()
        {
            var configuration = Configuration.Load(typeof(Foo));
            var setting = configuration.Settings.Single(s => s.Path == "Baz");
            Assert.IsTrue(setting.IsOptional);
        }

        [SmartConfig]
        private static class Foo
        {
            public static string Bar { get; set; }

            [Optional]
            public static string Baz{ get; set; }
        }
    }
}
