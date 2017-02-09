using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable InconsistentNaming

// ReSharper disable CheckNamespace
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SmartConfig.DataStores.SQLite.Tests
{
    using Reusable.Fuse;
    using Reusable.Fuse.Testing;
    using SQLite;

    [TestClass]
    public class SQLiteStoreTest
    {
        
        [TestMethod]
        public void RWR_TestConfig1()
        {
            Configuration.Builder
                .FromSQLite("name=configdb", builder => builder.TableName("Setting1"))
                .Select(typeof(TestConfig1));

            // Read 1

            TestConfig1.Utf8SettingDE.Verify().IsNotNullOrEmpty().IsEqual("äöüß");
            TestConfig1.Utf8SettingPL.Verify().IsNotNullOrEmpty().IsEqual("ąęśćżźó");

            TestConfig1.ArraySetting.Length.Verify().IsEqual(2);
            TestConfig1.ArraySetting[0].Verify().IsEqual(5);
            TestConfig1.ArraySetting[1].Verify().IsEqual(8);

            TestConfig1.DictionarySetting.Count.Verify().IsEqual(2);
            TestConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
            TestConfig1.DictionarySetting["bar"].Verify().IsEqual(34);
            //if (TestConfig1.DictionarySetting.Count == 3) { TestConfig1.DictionarySetting["baz"].Verify().IsEqual(55); }

            TestConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            TestConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Ignored value");


            // Modify

            TestConfig1.Utf8SettingDE = "äöüß--";
            TestConfig1.Utf8SettingPL = "ąęśćżźó--";

            TestConfig1.ArraySetting = new[] { 5, 8, 13 };
            TestConfig1.DictionarySetting["baz"] = 55;

            // Write 1

            Configuration.Save(typeof(TestConfig1));

            //  Read 2

            Configuration.Load(typeof(TestConfig1));

            TestConfig1.Utf8SettingDE.Verify().IsNotNullOrEmpty().IsEqual("äöüß--");
            TestConfig1.Utf8SettingPL.Verify().IsNotNullOrEmpty().IsEqual("ąęśćżźó--");

            TestConfig1.ArraySetting.Length.Verify().IsEqual(3);
            TestConfig1.ArraySetting[0].Verify().IsEqual(5);
            TestConfig1.ArraySetting[1].Verify().IsEqual(8);
            TestConfig1.ArraySetting[2].Verify().IsEqual(13);

            TestConfig1.DictionarySetting.Count.Verify().IsEqual(3);
            TestConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
            TestConfig1.DictionarySetting["bar"].Verify().IsEqual(34);
            TestConfig1.DictionarySetting["baz"].Verify().IsEqual(55);

            TestConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            TestConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Ignored value");
        }
    }
}
