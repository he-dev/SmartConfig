using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using Reusable.Fuse.Testing;
using Reusable.Fuse;

namespace SmartConfig.Core.Tests.Unit.Data
{
    [TestClass]
    public class SettingTest
    {
        [TestMethod]
        public void ctor_ObjectInitializer()
        {
            var setting = new Setting
            {
                Name = SettingPath.Parse("foo.bar[baz]"),
                Value = "waldo",
                Tags = new Dictionary<string, object> { ["Environment"] = "qux" },
                ["Config"] = "corge"
            };

            setting.Name.Verify().IsTrue(x => x == SettingPath.Parse("foo.bar[baz]"));

            setting.Tags.Count.Verify().IsEqual(2);
            setting.Tags["Environment"].Verify().IsTrue(x => x.Equals("qux"));
            setting.Tags["Config"].Verify().IsTrue(x => x.Equals("corge"));

            (setting["Environment"] as string).Verify().IsNotNullOrEmpty().IsEqual("qux");
            (setting["Config"] as string).Verify().IsNotNullOrEmpty().IsEqual("corge");

            setting.Value.Verify().IsNotNull().IsTrue(x => x.Equals("waldo"));

            setting.WeakId.Verify().IsNotNullOrEmpty().IsEqual("foo.bar/corge/qux");
            setting.StrongId.Verify().IsNotNullOrEmpty().IsEqual("foo.bar[baz]/corge/qux");

            //setting.NamespaceEquals("environment", "qux").Verify().IsTrue();
            //setting.IsLike(new Setting(
            //    new SettingUrn("foo.bar"),
            //    new Dictionary<string, object> { ["Environment"] = "qux" })
            //{ Config = "corge" }
            //).Verify().IsTrue();

            //setting.IsLike(new Setting(
            //    new SettingUrn("foo.baar"),
            //    new Dictionary<string, object> { ["Environment"] = "qux" })
            //{ Config = "corge" }
            //).Verify().IsFalse();
        }
    }
}
