using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Filters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class DataSourceTests
    {
        [TestMethod]
        public void RequiresCustomSettingSpecifiesFilter()
        {
            ExceptionAssert.Throws<FilterAttributeMissingException>(() =>
            {
                new TestDataSource<TestSettingWithoutFilter>();
            }, ex =>
            {
                Assert.AreEqual("Foo", ex.PropertyName);
                Assert.AreEqual(typeof(TestSettingWithoutFilter).FullName, ex.DeclaringTypeName);
            }, Assert.Fail);
        }

        [TestMethod]
        public void InitializesCustomKeyFilters()
        {
            var dataSource = new TestDataSource<TestSettingWithFilter>();
            Assert.AreEqual(1, dataSource.CustomKeyFilters.Count);

            var settingFilter = dataSource.CustomKeyFilters.First();
            Assert.AreEqual("Foo", settingFilter.Key);
            Assert.IsInstanceOfType(settingFilter.Value, typeof(StringFilter));
        }

        public class TestDataSource<TSetting> : DataSource<TSetting> where TSetting : Setting, new()
        {
            public override IReadOnlyCollection<Type> SupportedSettingValueTypes { get; }

            public override object Select(SettingKeyCollection keys)
            {
                throw new NotImplementedException();
            }

            public override void Update(SettingKeyCollection keys, object value)
            {
                throw new NotImplementedException();
            }
        }

        public class TestSettingWithoutFilter : Setting
        {
            public string Foo { get; set; }
        }

        public class TestSettingWithFilter : Setting
        {
            [Filter(typeof(StringFilter))]
            public string Foo { get; set; }
        }
    }
}
