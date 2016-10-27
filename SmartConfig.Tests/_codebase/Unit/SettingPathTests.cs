using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Testing;
using Reusable.Validations;

namespace SmartConfig.Core.Tests.Unit.SettingPath.Positive
{
    using SmartConfig;

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
            path.SettingNameEx.Verify().IsEqual("bar");
            path.ValueKey.Verify().IsNullOrEmpty();

            path.FullName.Verify().IsEqual("foo.bar");
            path.FullNameEx.Verify().IsEqual("foo.bar");
        }

        [TestMethod]
        public void CreateSettingPathShallow()
        {
            var path = new SettingPath(new[] { "foo" }, "baz");

            path.Count.Verify().IsEqual(1);

            path.SettingNamespace.Verify().IsNullOrEmpty();
            path.SettingName.Verify().IsEqual("foo");
            path.SettingNameEx.Verify().IsEqual("foo[baz]");
            path.ValueKey.Verify().IsEqual("baz");

            path.FullName.Verify().IsEqual("foo");
            path.FullNameEx.Verify().IsEqual("foo[baz]");
        }

        [TestMethod]
        public void CreateSettingPathWithValueKey()
        {
            var path = new SettingPath(new[] { "foo", "bar" }, "baz");

            path.Count.Verify().IsEqual(2);

            path.SettingNamespace.Verify().IsEqual("foo");
            path.SettingName.Verify().IsEqual("bar");
            path.SettingNameEx.Verify().IsEqual("bar[baz]");
            path.ValueKey.Verify().IsEqual("baz");

            path.FullName.Verify().IsEqual("foo.bar");
            path.FullNameEx.Verify().IsEqual("foo.bar[baz]");
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
            path.SettingNameEx.Verify().IsEqual("bar");
            path.ValueKey.Verify().IsNullOrEmpty();

            path.FullName.Verify().IsEqual("foo.bar");
            path.FullNameEx.Verify().IsEqual("foo.bar");
        }

        [TestMethod]
        public void CreateSettingPathWithValueKey()
        {
            var path = new SettingPath("foo.bar[baz]");

            path.Count.Verify().IsEqual(2);

            path.SettingNamespace.Verify().IsEqual("foo");
            path.SettingName.Verify().IsEqual("bar");
            path.SettingNameEx.Verify().IsEqual("bar[baz]");
            path.ValueKey.Verify().IsEqual("baz");

            path.FullName.Verify().IsEqual("foo.bar");
            path.FullNameEx.Verify().IsEqual("foo.bar[baz]");
        }
    }

    //[TestClass]
    //public class IsMatch
    //{
    //    [TestMethod]
    //    public void CreateSettingPathWithoutValueKey()
    //    {
    //        var path = new SettingPath("foo.bar");

    //        path.IsMatch("foo.bar").Validate().IsTrue();
    //        path.IsMatch("foo.bar[baz]").Validate().IsTrue();
    //        path.IsMatch("foo.baz").Validate().IsFalse();
    //    }       
    //}

    [TestClass]
    public class IsLike
    {
        [TestMethod]
        public void CreateSettingPathWithoutValueKey()
        {
            var path = new SettingPath("foo.bar");

            path.IsLike("foo.bar").Validate().IsTrue();
            path.IsLike("foo.bar[baz]").Validate().IsTrue();
            path.IsLike("foo.baz").Validate().IsFalse();
        }
    }

}
