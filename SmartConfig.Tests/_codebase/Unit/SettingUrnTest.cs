using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Testing;
using Reusable.Validations;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    [TestClass]
    public class SettingUrnTest
    {
        [TestMethod]
        public void ctor_CreateFromPath2WithoutKey()
        {
            var path = new SettingUrn(new[] { "foo", "bar" });

            path.Count.Verify().IsEqual(2);

            path.Namespace.Verify().IsEqual("foo");
            path.Name.Verify().IsEqual("bar");
            path.NameWithKey.Verify().IsEqual("bar");
            path.Key.Verify().IsNullOrEmpty();

            path.FullName.Verify().IsEqual("foo.bar");
            path.FullNameWithKey.Verify().IsEqual("foo.bar");
        }

        [TestMethod]
        public void ctor_CreateFromPath1WithKey()
        {
            var path = new SettingUrn(new[] { "foo" }, "baz");

            path.Count.Verify().IsEqual(1);

            path.Namespace.Verify().IsNullOrEmpty();
            path.Name.Verify().IsEqual("foo");
            path.NameWithKey.Verify().IsEqual("foo[baz]");
            path.Key.Verify().IsEqual("baz");

            path.FullName.Verify().IsEqual("foo");
            path.FullNameWithKey.Verify().IsEqual("foo[baz]");
        }

        [TestMethod]
        public void ctor_CreateFromPath2WithKey()
        {
            var path = new SettingUrn(new[] { "foo", "bar" }, "baz");

            path.Count.Verify().IsEqual(2);

            path.Namespace.Verify().IsEqual("foo");
            path.Name.Verify().IsEqual("bar");
            path.NameWithKey.Verify().IsEqual("bar[baz]");
            path.Key.Verify().IsEqual("baz");

            path.FullName.Verify().IsEqual("foo.bar");
            path.FullNameWithKey.Verify().IsEqual("foo.bar[baz]");
        }
    
        [TestMethod]
        public void ctor_CreateFromStringWithoutKey()
        {
            var path = new SettingUrn("foo.bar");

            path.Count.Verify().IsEqual(2);

            path.Namespace.Verify().IsEqual("foo");
            path.Name.Verify().IsEqual("bar");
            path.NameWithKey.Verify().IsEqual("bar");
            path.Key.Verify().IsNullOrEmpty();

            path.FullName.Verify().IsEqual("foo.bar");
            path.FullNameWithKey.Verify().IsEqual("foo.bar");
        }

        [TestMethod]
        public void ctor_CreateFromStringWithKey()
        {
            var path = new SettingUrn("foo.bar[baz]");

            path.Count.Verify().IsEqual(2);

            path.Namespace.Verify().IsEqual("foo");
            path.Name.Verify().IsEqual("bar");
            path.NameWithKey.Verify().IsEqual("bar[baz]");
            path.Key.Verify().IsEqual("baz");

            path.FullName.Verify().IsEqual("foo.bar");
            path.FullNameWithKey.Verify().IsEqual("foo.bar[baz]");
        }
           
        [TestMethod]
        public void IsLike_WithOrWithoutKey()
        {
            var path = new SettingUrn("foo.bar");

            path.IsLike("foo.bar").Validate().IsTrue();
            path.IsLike("foo.bar[baz]").Validate().IsTrue();
            path.IsLike("foo.baz").Validate().IsFalse();
        }
    }

}
