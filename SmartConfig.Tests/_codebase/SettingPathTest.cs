using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Fuse;
using Reusable.Fuse.Testing;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    [TestClass]
    public class SettingPathTest
    {
        [TestMethod]
        public void ctor_CreateFromPath2WithoutKey()
        {
            var path = new SettingPath(new[] { "foo", "bar" });

            path.Count.Verify().IsEqual(2);

            path.Namespace.Verify().IsEqual("foo");
            path.WeakName.Verify().IsEqual("bar");
            path.StrongName.Verify().IsEqual("bar");
            path.Key.Verify().IsNullOrEmpty();

            path.WeakFullName.Verify().IsEqual("foo.bar");
            path.StrongFullName.Verify().IsEqual("foo.bar");
        }

        [TestMethod]
        public void ctor_CreateFromPath1WithKey()
        {
            var path = new SettingPath("foo") { Key = "baz" };

            path.Count.Verify().IsEqual(1);

            path.Namespace.Verify().IsNullOrEmpty();
            path.WeakName.Verify().IsEqual("foo");
            path.StrongName.Verify().IsEqual("foo[baz]");
            path.Key.Verify().IsEqual("baz");

            path.WeakFullName.Verify().IsEqual("foo");
            path.StrongFullName.Verify().IsEqual("foo[baz]");
        }

        [TestMethod]
        public void ctor_CreateFromPath2WithKey()
        {
            var path = new SettingPath("foo", "bar") { Key = "baz" };

            path.Count.Verify().IsEqual(2);

            path.Namespace.Verify().IsEqual("foo");
            path.WeakName.Verify().IsEqual("bar");
            path.StrongName.Verify().IsEqual("bar[baz]");
            path.Key.Verify().IsEqual("baz");

            path.WeakFullName.Verify().IsEqual("foo.bar");
            path.StrongFullName.Verify().IsEqual("foo.bar[baz]");
        }

        [TestMethod]
        public void ctor_CreateFromStringWithoutKey()
        {
            var path = SettingPath.Parse("foo.bar");

            path.Count.Verify().IsEqual(2);

            path.Namespace.Verify().IsEqual("foo");
            path.WeakName.Verify().IsEqual("bar");
            path.StrongName.Verify().IsEqual("bar");
            path.Key.Verify().IsNullOrEmpty();

            path.WeakFullName.Verify().IsEqual("foo.bar");
            path.StrongFullName.Verify().IsEqual("foo.bar");
        }

        [TestMethod]
        public void ctor_CreateFromStringWithKey()
        {
            var path = SettingPath.Parse("foo.bar[baz]");

            path.Count.Verify().IsEqual(2);

            path.Namespace.Verify().IsEqual("foo");
            path.WeakName.Verify().IsEqual("bar");
            path.StrongName.Verify().IsEqual("bar[baz]");
            path.Key.Verify().IsEqual("baz");

            path.WeakFullName.Verify().IsEqual("foo.bar");
            path.StrongFullName.Verify().IsEqual("foo.bar[baz]");
        }

        [TestMethod]
        public void IsLike_WithOrWithoutKey()
        {
            var path = SettingPath.Parse("foo.bar");

            path.IsLike(SettingPath.Parse("foo.bar")).Verify().IsTrue();
            path.IsLike(SettingPath.Parse("foo.bar[baz]")).Verify().IsTrue();
            path.IsLike(SettingPath.Parse("foo.baz")).Verify().IsFalse();
        }
    }

}
