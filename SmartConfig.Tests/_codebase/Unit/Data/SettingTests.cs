using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Testing;
using Reusable.Validations;

namespace SmartConfig.Core.Tests.Unit.Data.Setting.Positive
{
    using SmartConfig;
    using SmartConfig.Data;

    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void CreateSetting()
        {
            var setting = new Setting(
                new SettingPath("foo.bar", "baz"),
                new Dictionary<string, object> { ["Environment"] = "qux" },
                "waldo")
            {
                Config = "corge"
            };

            setting.Name.Verify().IsTrue(x => x == new SettingPath("foo.bar", "baz"));
            setting.Namespaces.Count.Verify().IsEqual(2);
            setting.Namespaces.ContainsKey("Environment").Verify().IsTrue();
            setting.Namespaces["Environment"].Verify().IsTrue(x => x.Equals("qux"));
            setting.Config.Verify().IsNotNullOrEmpty().IsEqual("corge");
            setting.Value.Verify().IsNotNull().IsTrue(x => x.Equals("waldo"));

            setting.NamespaceEquals("environment", "qux").Verify().IsTrue();
            setting.IsLike(new Setting(
                new SettingPath("foo.bar"),
                new Dictionary<string, object> { ["Environment"] = "qux" })
            { Config = "corge" }
            ).Verify().IsTrue();

            setting.IsLike(new Setting(
                new SettingPath("foo.baar"),
                new Dictionary<string, object> { ["Environment"] = "qux" })
            { Config = "corge" }
            ).Verify().IsFalse();
        }
    }
}
