using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.DataAnnotations;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.Collections.SettingCollectionTests
{
    [TestClass]
    public class CreateTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresConfigurationInfo()
        {
            SettingCollection.Create(null);
        }

        [TestMethod]
        public void CreatesSettingCollection()
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
        }
    }

}