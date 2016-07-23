using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

namespace SmartConfig.Core.Tests.SettingPathUnit.Positive
{
    [TestClass]
    public class ctor_IList_String
    {
        [TestMethod]
        public void CreateSettingPathWithoutValueKey()
        {
            var path = new SettingPath(new[] { "foo", "bar" });

            path.Count.Verify().IsEqual(2);

            path.SettingNamespace.Verify().IsEqual("foo");
            path.SettingName.Verify().IsEqual("bar");
            path.SettingNameWithValueKey.Verify().IsEqual("bar");
            path.ValueKey.Verify().IsNullOrEmpty();

            path.ToString().Verify().IsEqual("foo.bar");
            path.ToStringWithValueKey().Verify().IsEqual("foo.bar");
        }

        [TestMethod]
        public void CreateSettingPathWithValueKey()
        {
            var path = new SettingPath(new[] { "foo", "bar" }, "baz");

            path.Count.Verify().IsEqual(2);

            path.SettingNamespace.Verify().IsEqual("foo");
            path.SettingName.Verify().IsEqual("bar");
            path.SettingNameWithValueKey.Verify().IsEqual("bar[baz]");
            path.ValueKey.Verify().IsEqual("baz");

            path.ToString().Verify().IsEqual("foo.bar");
            path.ToStringWithValueKey().Verify().IsEqual("foo.bar[baz]");
        }
    }

    [TestClass]
    public class ctor_String_String
    {
        [TestMethod]
        public void CreateSettingPathWithoutValueKey()
        {
            var path = new SettingPath("foo.bar");

            path.Count.Verify().IsEqual(2);

            path.SettingNamespace.Verify().IsEqual("foo");
            path.SettingName.Verify().IsEqual("bar");
            path.SettingNameWithValueKey.Verify().IsEqual("bar");
            path.ValueKey.Verify().IsNullOrEmpty();

            path.ToString().Verify().IsEqual("foo.bar");
            path.ToStringWithValueKey().Verify().IsEqual("foo.bar");
        }

        [TestMethod]
        public void CreateSettingPathWithValueKey()
        {
            var path = new SettingPath("foo.bar[baz]");

            path.Count.Verify().IsEqual(2);

            path.SettingNamespace.Verify().IsEqual("foo");
            path.SettingName.Verify().IsEqual("bar");
            path.SettingNameWithValueKey.Verify().IsEqual("bar[baz]");
            path.ValueKey.Verify().IsEqual("baz");

            path.ToString().Verify().IsEqual("foo.bar");
            path.ToStringWithValueKey().Verify().IsEqual("foo.bar[baz]");
        }
    }

}
