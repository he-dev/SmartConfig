using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.DataStores;
using Reusable.Fuse.Testing;
using Reusable.Fuse;

namespace SmartConfig.Core.Tests.Unit.DataStores
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
        public void GetSettings_ByName0()
        {
            var store = new MemoryStore();

            var settings = store.GetSettings(new Setting { Name = SettingUrn.Parse("baz") });
            settings.Count().Verify().IsEqual(0);
        }

        [TestMethod]
        public void GetSettings_ByName2()
        {
            var store = new MemoryStore
            {
                { "foo", "bar" },
                { "baz", "qux" }
            };

            var settings = store.GetSettings(new Setting { Name = SettingUrn.Parse("baz") });
            settings.Count().Verify().IsEqual(1);
            settings.First().Value.ToString().Verify().IsEqual("qux");
        }

        [TestMethod]
        public void GetSettings_ByNameAndKey()
        {
            var store = new MemoryStore
            {
                { "foo[a]", "bar" },
                { "foo[b]", "qux" },
                { "bar[b]", "qux" }
            };

            var settings = store.GetSettings(new Setting { Name = SettingUrn.Parse("foo") });
            settings.Count().Verify().IsEqual(2);
            //settings.First().Value.ToString().Verify().IsEqual("qux");
        }
    }
}
