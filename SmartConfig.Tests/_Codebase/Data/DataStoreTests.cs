using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Filters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Core.Tests.Data.DataStoreTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void RequiresCustomSettingSpecifiesFilter()
        {
            ExceptionAssert.Throws<FilterAttributeMissingException>(() =>
            {
                new TestDataStore<TestSettingWithoutFilter>();
            }, ex =>
            {
                Assert.AreEqual("Foo", ex.PropertyName);
                Assert.AreEqual(typeof(TestSettingWithoutFilter).FullName, ex.DeclaringTypeName);
            }, Assert.Fail);
        }

        [TestMethod]
        public void InitializesCustomKeyFilters()
        {
            var dataSource = new TestDataStore<TestSettingWithFilter>();
            Assert.AreEqual(1, dataSource.SettingFilters.Count);

            var settingFilter = dataSource.SettingFilters.First();
            Assert.AreEqual("Foo", settingFilter.Key);
            Assert.IsInstanceOfType(settingFilter.Value, typeof(StringFilter));
        }

        public class TestDataStore<TSetting> : DataStore<TSetting> where TSetting : BasicSetting, new()
        {
            public override IReadOnlyCollection<Type> SupportedSettingDataTypes { get; } = new ReadOnlyCollection<Type>(Enumerable.Empty<Type>().ToList());

            public override object Select(CompoundSettingKey keys)
            {
                throw new NotImplementedException();
            }

            public override void Update(CompoundSettingKey keys, object value)
            {
                throw new NotImplementedException();
            }
        }

        public class TestSettingWithoutFilter : BasicSetting
        {
            public string Foo { get; set; }
        }

        public class TestSettingWithFilter : BasicSetting
        {
            [SettingFilter(typeof(StringFilter))]
            public string Foo { get; set; }
        }
    }

    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void ReceivesCompundKeyWithoutModelName()
        {
            Configuration.Load(typeof(Foo)).From(new TestDataStore<BasicSetting>
            {
                SelectFunc = key =>
                {
                    Assert.IsTrue(key.Count == 1);
                    Assert.AreEqual("Name", key.NameKey.Name);
                    Assert.AreEqual("Bar", key.NameKey.Value);
                    Assert.IsFalse(key.CustomKeys.Any());
                    return null;
                }
            });
        }

        [TestMethod]
        public void ReceivesCompundKeyWithModelName()
        {
            Configuration.Load(typeof(Foo2)).From(new TestDataStore<BasicSetting>
            {
                SelectFunc = key =>
                {
                    Assert.IsTrue(key.Count == 1);
                    Assert.AreEqual("Name", key.NameKey.Name);
                    Assert.AreEqual("baz.Bar2", key.NameKey.Value);
                    Assert.IsFalse(key.CustomKeys.Any());
                    return null;
                }
            });
        }

        [TestMethod]
        public void ReceivesCompundKeyWithFilters()
        {
            Configuration.Load(typeof(Foo)).HasAdditionalKey("Qux", "bax").From(new TestDataStore<CustomTestSetting>
            {
                SelectFunc = key =>
                {
                    Assert.IsTrue(key.Count == 2);
                    Assert.AreEqual("Name", key.NameKey.Name);
                    Assert.AreEqual("Bar", key.NameKey.Value);
                    Assert.IsTrue(key.CustomKeys.Count() == 1);
                    Assert.AreEqual("Qux", key.CustomKeys.First().Name);
                    Assert.AreEqual("bax", key.CustomKeys.First().Value);
                    return null;
                }
            });
        }

        [SmartConfig]
        static class Foo
        {
            [Optional]
            static public string Bar { get; set; }
        }

        [SmartConfig]
        [SettingName("baz")]
        static class Foo2
        {
            [Optional]
            static public string Bar2 { get; set; }
        }
    }

    class TestDataStore<TSetting> : DataStore<TSetting> where TSetting : BasicSetting, new()
    {
        public override IReadOnlyCollection<Type> SupportedSettingDataTypes { get; }

        public Func<CompoundSettingKey, object> SelectFunc { get; set; }

        public Action<CompoundSettingKey, object> UpdateFunc { get; set; }

        public override object Select(CompoundSettingKey key)
        {
            return SelectFunc(key);
        }

        public override void Update(CompoundSettingKey key, object value)
        {
            UpdateFunc(key, value);
        }
    }

    class CustomTestSetting : BasicSetting
    {
        [SettingFilter(typeof(StringFilter))]
        public string Qux { get; set; }
    }
}
