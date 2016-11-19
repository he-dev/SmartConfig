using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Testing;
using Reusable.Validations;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests.Data
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void CreateSetting()
        {
            var setting = new Setting
            {
                Name = SettingUrn.Parse("foo.bar[baz]"),
                Value = "waldo",
                Attributes = new Dictionary<string, object> {["Environment"] = "qux"},
                ["Config"] = "corge"
            };

            setting.Name.Verify().IsTrue(x => x == SettingUrn.Parse("foo.bar[baz]"));
            setting.Attributes.Count.Verify().IsEqual(2);
            setting.Attributes.ContainsKey("Environment").Verify().IsTrue();
            setting.Attributes["Environment"].Verify().IsTrue(x => x.Equals("qux"));
            (setting["Config"] as string).Verify().IsNotNullOrEmpty().IsEqual("corge");
            setting.Value.Verify().IsNotNull().IsTrue(x => x.Equals("waldo"));

            setting.NamespaceEquals("environment", "qux").Verify().IsTrue();
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
