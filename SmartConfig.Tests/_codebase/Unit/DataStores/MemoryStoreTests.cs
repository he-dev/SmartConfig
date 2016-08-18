using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartUtilities.Frameworks.InlineValidation;
using SmartUtilities.Frameworks.InlineValidation.Testing;

namespace SmartConfig.Core.Tests.Unit.DataStores.MemoryStore.Positive
{
    using SmartConfig;
    using SmartConfig.DataStores;

    [TestClass]
    public class Add_String_Object
    {
        [TestMethod]
        public void AddSettings()
        {
            var store = new MemoryStore
            {
                { "foo", "bar" },
                { "baz", "qux" }
            };

            store.Count().Verify().IsEqual(2);
        }
    }

    [TestClass]
    public class GetSettings
    {
        [TestMethod]
        public void GetEmptySettingsByName()
        {
            var store = new MemoryStore();

            var settings = store.GetSettings(new Setting { Name = new SettingPath("baz") });
            settings.Count.Verify().IsEqual(0);
        }

        [TestMethod]
        public void GetSettingsByName()
        {
            var store = new MemoryStore
            {
                { "foo", "bar" },
                { "baz", "qux" }
            };

            var settings = store.GetSettings(new Setting { Name = "baz" });
            settings.Count.Verify().IsEqual(1);
            settings.First().Value.ToString().Verify().IsEqual("qux");
        }

        [TestMethod]
        public void GetSettingsByNameAndKey()
        {
            var store = new MemoryStore
            {
                { "foo[a]", "bar" },
                { "foo[b]", "qux" },
                { "bar[b]", "qux" }
            };

            var settings = store.GetSettings(new Setting { Name = "foo" });
            settings.Count.Verify().IsEqual(2);
            //settings.First().Value.ToString().Verify().IsEqual("qux");
        }
    }

    [TestClass]
    public class SaveSetting
    {

    }
}
