using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.DataStores.AppConfig.Tests.TestData;

namespace SmartConfig.DataStores.AppConfig.Tests.Integration
{
    [TestClass]
    public class AppSettingsStoreTest
    {
        [TestMethod]
        public void Load_SimpleConfig()
        {
            Configuration.Loader.From(new AppSettingsStore()).Select(typeof(FullConfig1));

            FullConfig1.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foo");
            FullConfig1.ArraySetting.Length.Verify().IsBetweenOrEqual(2, 3);
            FullConfig1.ArraySetting[0].Verify().IsEqual(5);
            FullConfig1.ArraySetting[1].Verify().IsEqual(8);
            if (FullConfig1.ArraySetting.Length == 3)
            {
                FullConfig1.ArraySetting[2].Verify().IsEqual(13);
            }
            FullConfig1.DictionarySetting.Count.Verify().IsBetweenOrEqual(2, 3);
            FullConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
            FullConfig1.DictionarySetting["bar"].Verify().IsEqual(34);
            if (FullConfig1.DictionarySetting.Count == 3)
            {
                FullConfig1.DictionarySetting["baz"].Verify().IsEqual(55);
            }
            FullConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            FullConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");

            if (FullConfig1.ArraySetting.Length == 2) FullConfig1.ArraySetting = new[] { 5, 8, 13 };
            else if (FullConfig1.ArraySetting.Length == 3) FullConfig1.ArraySetting = new[] { 5, 8 };

            if (FullConfig1.DictionarySetting.Count == 2) FullConfig1.DictionarySetting["baz"] = 55;
            else if (FullConfig1.DictionarySetting.Count == 3) FullConfig1.DictionarySetting.Remove("baz");

            Configuration.Save(typeof(FullConfig1));

        }

        [TestMethod]
        public void Load_ConfigWithNameAsPath()
        {
            Configuration.Loader.From(new AppSettingsStore()).Select(typeof(FullConfig2));

            FullConfig2.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foox");
            FullConfig2.ArraySetting.Length.Verify().IsEqual(2);
            FullConfig2.DictionarySetting.Count.Verify().IsEqual(2);
            FullConfig2.NestedConfig.StringSetting.Verify().IsEqual("Barx");
            FullConfig2.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }
    }

}

