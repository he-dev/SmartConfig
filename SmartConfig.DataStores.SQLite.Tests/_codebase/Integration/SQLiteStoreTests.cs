using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Testing;
using Reusable.Validations;

// ReSharper disable CheckNamespace
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SmartConfig.DataStores.SQLite.Tests.Integration.SQLiteStore.Positive
{
    using SQLite;

    [TestClass]
    public class FullTests
    {
        [TestMethod]
        public void GetSettingsSimple()
        {
            Configuration.Load
                .From(new SQLiteStore("name=configdb", builder => builder.TableName("Setting1")))
                .Select(typeof(FullConfig1));

            // Verfiy FullConfig1 values.

            FullConfig1.Utf8SettingDE.Verify().IsNotNullOrEmpty().IsEqual("äöüß");
            FullConfig1.Utf8SettingPL.Verify().IsNotNullOrEmpty().IsEqual("ąęśćżźó");
            FullConfig1.ArraySetting.Length.Verify().IsBetweenOrEqual(2, 3);
            FullConfig1.ArraySetting.Length.Verify().IsBetweenOrEqual(2, 3);
            FullConfig1.ArraySetting.Contains(5).Verify().IsTrue();
            FullConfig1.ArraySetting.Contains(8).Verify().IsTrue();
            if (FullConfig1.ArraySetting.Length == 3) { FullConfig1.ArraySetting.Contains(13).Verify().IsTrue(); }
            FullConfig1.DictionarySetting.Count.Verify().IsBetweenOrEqual(2, 3);
            FullConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
            FullConfig1.DictionarySetting["bar"].Verify().IsEqual(34);
            if (FullConfig1.DictionarySetting.Count == 3) { FullConfig1.DictionarySetting["baz"].Verify().IsEqual(55); }
            FullConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            FullConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");

            // Change, save & reload FullConfig1.

            if (FullConfig1.ArraySetting.Length == 2) { FullConfig1.ArraySetting = new[] { 5, 8, 13 }; }
            else if (FullConfig1.ArraySetting.Length == 3) { FullConfig1.ArraySetting = new[] { 5, 8 }; }

            if (FullConfig1.DictionarySetting.Count == 2) { FullConfig1.DictionarySetting["baz"] = 55; }
            else if (FullConfig1.DictionarySetting.Count == 3) { FullConfig1.DictionarySetting.Remove("baz"); }

            Configuration.Save(typeof(FullConfig1));
            Configuration.TryReload(typeof(FullConfig1));

            // Verfiy FullConfig1 values agian.

            FullConfig1.Utf8SettingDE.Verify().IsNotNullOrEmpty().IsEqual("äöüß");
            FullConfig1.Utf8SettingPL.Verify().IsNotNullOrEmpty().IsEqual("ąęśćżźó");
            FullConfig1.ArraySetting.Length.Verify().IsBetweenOrEqual(2, 3);
            FullConfig1.ArraySetting[0].Verify().IsEqual(5);
            FullConfig1.ArraySetting[1].Verify().IsEqual(8);
            if (FullConfig1.ArraySetting.Length == 3) { FullConfig1.ArraySetting[2].Verify().IsEqual(13); }
            FullConfig1.DictionarySetting.Count.Verify().IsBetweenOrEqual(2, 3);
            FullConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
            FullConfig1.DictionarySetting["bar"].Verify().IsEqual(34);
            if (FullConfig1.DictionarySetting.Count == 3) { FullConfig1.DictionarySetting["baz"].Verify().IsEqual(55); }
            FullConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            FullConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }
    }
}
