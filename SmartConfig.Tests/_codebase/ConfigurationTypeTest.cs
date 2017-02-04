using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data.Annotations;

namespace SmartConfig.Core.Tests
{
    [TestClass]
    public class ConfigurationTypeTest
    {
        [TestMethod]
        public void GetSettingProperties_FromNestedWithIgnore()
        {
            var settings = ConfigurationType.GetSettingProperties(typeof(Foo)).ToList();
            settings.Count.Verify().IsEqual(3);
        }

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
    }
}
