using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    [TestClass]
    public class WeakSettingComparerTest
    {
        [TestMethod]
        public void Equals_SameSettingsWithoutTags_True()
        {
            var s1 = new Setting
            {
                Name = SettingPath.Parse("S1"),
                Value = "V1"
            };
            var s2 = new Setting
            {
                Name = SettingPath.Parse("S1"),
                Value = "V2"
            };
            var comparer = new WeakSettingComparer();
            comparer.Equals(s1, s2).Verify().IsTrue();
        }

        [TestMethod]
        public void Equals_SameSettingsWithTags_True()
        {
            var s1 = new Setting
            {
                Name = SettingPath.Parse("S1"),
                Value = "V1",
                Tags = { { "Tag2", "T2" }, { "Tag1", "T1" } }
            };
            var s2 = new Setting
            {
                Name = SettingPath.Parse("S1"),
                Value = "V2",
                Tags = { { "Tag2", "T2" }, { "Tag1", "T1" } }
            };
            var comparer = new WeakSettingComparer();
            comparer.Equals(s1, s2).Verify().IsTrue();
        }

        [TestMethod]
        public void Equals_SameSettingsWithDifferentTags_False()
        {
            var s1 = new Setting
            {
                Name = SettingPath.Parse("S1"),
                Value = "V1",
                Tags = { { "Tag2", "T2" }, { "Tag1", "T1" } }
            };
            var s2 = new Setting
            {
                Name = SettingPath.Parse("S1"),
                Value = "V2",
                Tags = { { "Tag3", "T3" }, { "Tag1", "T1" } }
            };
            var comparer = new WeakSettingComparer();
            comparer.Equals(s1, s2).Verify().IsFalse();
        }
    }
}
