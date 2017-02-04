using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests.Data
{
    [TestClass]
    public class SettingTest
    {
        [TestMethod]
        public void WeakId_NameOnly_NameOnly()
        {
            var setting = new Setting
            {
                Name = SettingPath.Parse("Setting1"),
                Value = "Value1"
            };
            setting.WeakId.Verify().IsEqual("[Setting1]");
        }

        [TestMethod]
        public void WeakId_NameWithKey_NameOnly()
        {
            var setting = new Setting
            {
                Name = SettingPath.Parse("Setting1[0]"),
                Value = "Setting1-Value"
            };
            setting.WeakId.Verify().IsEqual("[Setting1]");
        }

        [TestMethod]
        public void WeakId_WithTags_NameAndTags()
        {
            var setting = new Setting
            {
                Name = SettingPath.Parse("Setting1"),
                Value = "Value1",
                Tags = { { "Tag2", "TagValue2" }, { "Tag1", "TagValue1" } }
            };
            setting.WeakId.Verify().IsEqual("[Setting1, TagValue1, TagValue2]");
        }       
    }
}
