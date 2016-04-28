using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartConfig.Filters;
using SmartUtilities.ObjectConverters.DataAnnotations;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Core.Tests.Data.DataStoreTests
{
    [TestClass]
    public class ctor
    {
        //[TestMethod]
        //public void RequiresCustomSettingSpecifiesFilter()
        //{
        //    ExceptionAssert.Throws<FilterAttributeMissingException>(() =>
        //    {
        //        //new TestDataStore<TestSettingWithoutFilter>();
        //        new TestSettingWithoutFilter();
        //    }, ex =>
        //    {
        //        Assert.AreEqual("Foo", ex.Property);
        //        Assert.AreEqual(typeof(TestSettingWithoutFilter).Main, ex.SettingType);
        //    }, Assert.Fail);
        //}

        [TestMethod]
        public void InitializesCustomKeyFilters()
        {
            var dataSource = new TestDataStore<TestSettingWithFilter>();
            Assert.AreEqual(1, dataSource.CustomKeyFilters.Count);

            var settingFilter = dataSource.CustomKeyFilters.First();
            Assert.AreEqual("Foo", settingFilter.Key);
            Assert.IsInstanceOfType(settingFilter.Value, typeof(StringFilter));
        }

        public class TestDataStore<TSetting> : DataStore<TSetting> where TSetting : BasicSetting, new()
        {
            public override IReadOnlyCollection<Type> SerializationTypes { get; } = new ReadOnlyCollection<Type>(Enumerable.Empty<Type>().ToList());

            public override object Select(SettingKey key)
            {
                throw new NotImplementedException();
            }

            public override void Update(SettingKey key, object value)
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
        public void ReceivesSettingKeyWithoutConfigurationName()
        {
            Configuration.Load(typeof(Foo)).From(new TestDataStore<BasicSetting>
            {
                SelectFunc = key =>
                {
                    Assert.IsTrue(key.Count() == 1);
                    Assert.AreEqual(BasicSetting.MainKeyName, key.Main.Key);
                    Assert.AreEqual("Bar", key.Main.Value.ToString());
                    Assert.IsFalse(key.CustomKeys.Any());
                    return null;
                }
            });
        }

        [TestMethod]
        public void ReceivesSettingKeyWithConfigurationName()
        {
            Configuration.Load(typeof(Foo2)).From(new TestDataStore<BasicSetting>
            {
                SelectFunc = key =>
                {
                    Assert.IsTrue(key.Count() == 1);
                    Assert.AreEqual(BasicSetting.MainKeyName, key.Main.Key);
                    Assert.AreEqual("baz.Bar2", key.Main.Value);
                    Assert.IsFalse(key.CustomKeys.Any());
                    return null;
                }
            });
        }

        [TestMethod]
        public void ReceivesSettingKeyWithFilters()
        {
            Configuration
                .Load(typeof(Foo))
                .From(new TestDataStore<CustomTestSetting>
                {
                    SelectFunc = key =>
                    {
                        Assert.IsTrue(key.Count() == 2);
                        Assert.AreEqual(BasicSetting.MainKeyName, key.Main.Key);
                        Assert.AreEqual("Bar", key.Main.Value);
                        Assert.IsTrue(key.CustomKeys.Count() == 1);
                        Assert.AreEqual("Qux", key.CustomKeys.First().Key);
                        Assert.AreEqual("bax", key.CustomKeys.First().Value);
                        return null;
                    }
                }, dataStore =>
                {
                    dataStore.SetCustomKey("Qux", "bax");
                });
        }

        [SmartConfig]
        private static class Foo
        {
            [Optional]
            public static string Bar { get; set; }
        }

        [SmartConfig]
        [CustomName("baz")]
        private static class Foo2
        {
            [Optional]
            public static string Bar2 { get; set; }
        }
    }

    internal class TestDataStore<TSetting> : DataStore<TSetting> where TSetting : BasicSetting, new()
    {
        public override IReadOnlyCollection<Type> SerializationTypes { get; }

        public Func<SettingKey, object> SelectFunc { get; set; }

        public Action<SettingKey, object> UpdateFunc { get; set; }

        public override object Select(SettingKey key)
        {
            return SelectFunc(key);
        }

        public override void Update(SettingKey key, object value)
        {
            UpdateFunc(key, value);
        }
    }

    internal class CustomTestSetting : BasicSetting
    {
        [SettingFilter(typeof(StringFilter))]
        public string Qux { get; set; }
    }
}
