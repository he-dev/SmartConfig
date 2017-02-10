using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Data.Annotations;
using Reusable.Fuse.Testing;
using Reusable.Fuse;
using SmartConfig.Data.Annotations;
using SmartConfig.DataStores.SqlServer;
using SmartConfig.DataStores.Tests.Common;

namespace SmartConfig.DataStores.SqlServer.Tests
{
    // ReSharper disable InconsistentNaming

    [TestClass]
    public class ConfigurationTest : ConfigurationTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            DataStore = new SqlServerStore("name=SmartConfigTest", configure => configure.TableName("Setting1"));
        }

        //[TestMethod]
        //public void RWR_TestConfig1()
        //{
        //    Configuration.Builder
        //        .FromSqlServer("name=SmartConfigTest", configure => configure.TableName("Setting1"))
        //        .Select(typeof(TestConfig1));

        //    TestConfig1.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foo");

        //    TestConfig1.ArraySetting.Length.Verify().IsBetweenOrEqual(2, 3);
        //    TestConfig1.ArraySetting.Contains(5).Verify().IsTrue();
        //    TestConfig1.ArraySetting.Contains(8).Verify().IsTrue();
        //    if (TestConfig1.ArraySetting.Length == 3)
        //    {
        //        TestConfig1.ArraySetting.Contains(13).Verify().IsTrue();
        //    }
        //    TestConfig1.DictionarySetting.Count.Verify().IsBetweenOrEqual(2, 3);
        //    TestConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
        //    TestConfig1.DictionarySetting["bar"].Verify().IsEqual(34);
        //    if (TestConfig1.DictionarySetting.Count == 3)
        //    {
        //        TestConfig1.DictionarySetting["baz"].Verify().IsEqual(55);
        //    }
        //    TestConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
        //    TestConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");

        //    if (TestConfig1.ArraySetting.Length == 2) TestConfig1.ArraySetting = new[] { 5, 8, 13 };
        //    else if (TestConfig1.ArraySetting.Length == 3) TestConfig1.ArraySetting = new[] { 5, 8 };

        //    if (TestConfig1.DictionarySetting.Count == 2) TestConfig1.DictionarySetting["baz"] = 55;
        //    else if (TestConfig1.DictionarySetting.Count == 3) TestConfig1.DictionarySetting.Remove("baz");

        //    Configuration.Save(typeof(TestConfig1));
        //}        
    }
}