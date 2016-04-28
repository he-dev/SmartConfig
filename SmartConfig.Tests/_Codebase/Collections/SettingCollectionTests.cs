using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.DataAnnotations;
using SmartUtilities.UnitTesting;


// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.Collections.SettingCollectionTests
{
    [TestClass]
    public class CreateTests
    {
        [TestMethod]
        public void RequiresConfigurationInfo()
        {
            ExceptionAssert.Throws<ArgumentException>(() =>
            {
                SettingCollection.Create(null);
            }, null, Assert.Fail);
        }

        [TestMethod]
        public void IgnoresNotMappedTypes()
        {
            var configuration = Configuration.Load(typeof(Foo));
            Assert.AreEqual(3, configuration.Settings.Count);
        }

        [SmartConfig]
        private static class Foo
        {
            public static string Bar1 { get; set; }
            public static string Bar2 { get; set; }

            public static class SubFoo
            {
                public static string Baz1 { get; set; }

                [SmartUtilities.ObjectConverters.DataAnnotations.Ignore]
                public static string Baz2 { get; set; }
            }

            [SmartUtilities.ObjectConverters.DataAnnotations.Ignore]
            public static class SubBaz
            {
                public static string Quux { get; set; }
            }
        }
    }

}