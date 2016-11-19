using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Validations;
using SmartConfig.DataAnnotations;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.Collections.SettingCollection.Positive
{
    [TestClass]
    public class ConfigurationReflectionTest
    {
        [TestMethod]
        public void GetSettingProperties_FromNestedWithIgnore()
        {
            var settings = typeof(Foo).GetSettingProperties();
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

                [Reusable.Data.DataAnnotations.Ignore]
                public static string Baz2 { get; set; }
            }

            [Reusable.Data.DataAnnotations.Ignore]
            public static class SubBaz
            {
                public static string Quux { get; set; }

                public static class SubSubBaz
                {
                    public static string SubQuux { get; set; }
                }
            }
        }
    }
}