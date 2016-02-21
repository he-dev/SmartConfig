using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.Collections.SettingCollectionTests
{
    [TestClass]
    public class GetSettingGroupsTests
    {
        [TestMethod]
        [ExpectedException(typeof(TypeNotStaticException))]
        public void RequiresTypeIsStatic()
        {
            SettingCollection.GetSettingGroups(typeof(Bar), new List<Type>());
        }

        [TestMethod]
        public void GetsOnlyRelevantTypes()
        {
            var settingGroups = SettingCollection.GetSettingGroups(typeof(Foo));
            Assert.AreEqual(6, settingGroups.Count);

            // make sure the types that shouldn't be there aren't there
            Assert.IsNull(settingGroups.SingleOrDefault(t => t == typeof(Foo.SubFoo2.Baz)));
        }

        [SmartConfig]
        static class Foo
        {
            //[SmartConfigProperties]
            //public static class Bar { }

            public static class SubFoo1 { }

            public static class SubFoo2
            {
                public static class SubSubFoo1
                {
                    public static class SubSubSubFoo1 { }
                }

                public static class SubSubFoo2 { }

                [Ignore]
                public static class Baz { }
            }
        }

        [SmartConfig]
        private static class Bar
        {
            public static class SubBar1
            {
            }

            public class SubBar2
            {
            }
        }
    }

    [TestClass]
    public class Create
    {
        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void RequiresConfigurationInfo()
        //{
        //    ((Configuration)null).GetSettings();
        //}

        [TestMethod]
        public void CreatesSettingCollection()
        {
            var configuration = new Configuration(typeof(Foo));
            var settingInfos = SettingCollection.Create(configuration);
            Assert.AreEqual(3, settingInfos.Count);
        }

        [SmartConfig]
        static class Foo
        {
            static public string Bar1 { get; set; }
            static public string Bar2 { get; set; }

            public static class SubFoo
            {
                static public string Baz1 { get; set; }

                [Ignore]
                static public string Baz2 { get; set; }
            }
        }
    }

}