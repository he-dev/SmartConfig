using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data.Annotations;

namespace SmartConfig.Core.Tests.ConfigurationReflectionTestData
{
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

    [SmartConfig(Name = "Corge")]
    internal static class Foo2
    {
        public static string Bar { get; set; }

        [Rename("Qux")]
        public static string Baz { get; set; }
    }

    internal static class MissingAttributeConfig { }

    [SmartConfig]
    internal static class EmpyConfig { }

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
}