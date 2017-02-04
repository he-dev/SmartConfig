using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;
using SmartConfig.DataStores;

namespace SmartConfig.Core.Tests.DataStores
{
    [TestClass]
    public class MemoryStoreTest
    {
        [TestMethod]
        public void Add_ObjectInitializer()
        {
            var store = new MemoryStore
            {
                { "foo", "bar" },
                { "baz", "qux" }
            };

            store.Count().Verify().IsEqual(2);
        }

        [TestMethod]
        public void GetSettings_NameDoesNotExist_EmptyResult()
        {
            var store = new MemoryStore();

            var settings = store.GetSettings(new Setting { Name = SettingPath.Parse("baz") }).ToList();
            settings.Count().Verify().IsEqual(0);
        }

        [TestMethod]
        public void GetSettings_NameExists_OneSetting()
        {
            var store = new MemoryStore
            {
                { "foo", "bar" },
                { "baz", "qux" }
            };

            var settings = store.GetSettings(new Setting { Name = SettingPath.Parse("baz") }).ToList();
            settings.Count.Verify().IsEqual(1);
            settings.First().Value.ToString().Verify().IsEqual("qux");
        }

        [TestMethod]
        public void GetSettings_NameAndKeyExist_TwoSettings()
        {
            var store = new MemoryStore
            {
                { "foo[a]", "bar" },
                { "foo[b]", "qux" },
                { "bar[b]", "qux" }
            };

            var settings = store.GetSettings(new Setting { Name = SettingPath.Parse("foo") }).ToList();
            settings.Count().Verify().IsEqual(2);
            settings[0].Value.ToString().Verify().IsEqual("bar");
            settings[1].Value.ToString().Verify().IsEqual("qux");
        }

        // TODO add SaveSettings tests
    }
}
