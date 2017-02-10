using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;

// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SmartConfig.DataStores.Tests.Common
{
    public class ConfigurationTestBase
    {
        protected DataStore DataStore { get; set; }

        [TestMethod]
        public void RWR_TestConfig1()
        {
            Configuration.Builder.From(DataStore).Select(typeof(TestConfig1));

            #region Read 1

            TestConfig1.Utf8SettingDE.Verify().IsNotNullOrEmpty().IsEqual("äöüß");
            TestConfig1.Utf8SettingPL.Verify().IsNotNullOrEmpty().IsEqual("ąęśćżźó");

            TestConfig1.ArraySetting.Length.Verify().IsEqual(2);
            TestConfig1.ArraySetting[0].Verify().IsEqual(5);
            TestConfig1.ArraySetting[1].Verify().IsEqual(8);

            TestConfig1.DictionarySetting.Count.Verify().IsEqual(2);
            TestConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
            TestConfig1.DictionarySetting["bar"].Verify().IsEqual(34);

            TestConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            TestConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Ignored value");

            #endregion

            #region Modify & Write

            TestConfig1.Utf8SettingDE = "äöüß--";
            TestConfig1.Utf8SettingPL = "ąęśćżźó--";

            TestConfig1.ArraySetting = new[] { 5, 8, 13 };
            TestConfig1.DictionarySetting["baz"] = 55;

            Configuration.Save(typeof(TestConfig1));

            #endregion

            #region Read 2

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

            #endregion
        }
    }
}
