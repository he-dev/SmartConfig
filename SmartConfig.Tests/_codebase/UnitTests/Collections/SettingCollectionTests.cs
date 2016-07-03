using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.DataAnnotations;
using SmartUtilities.TypeFramework;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;


// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.Collections.SettingCollectionTests
{
    [TestClass]
    public class From_UseCases
    {
        [TestMethod]
        public void CreateSettingCollection()
        {
            var settings = SettingCollection.From(typeof(Foo));
            settings.Count.Validate().IsEqual(3);
        }

        [SmartConfig]
        private static class Foo
        {
            public static string Bar1 { get; set; }
            public static string Bar2 { get; set; }

            public static class SubFoo
            {
                public static string Baz1 { get; set; }

                [SmartUtilities.DataAnnotations.Ignore]
                public static string Baz2 { get; set; }
            }

            [SmartUtilities.DataAnnotations.Ignore]
            public static class SubBaz
            {
                public static string Quux { get; set; }
            }
        }
    }

}