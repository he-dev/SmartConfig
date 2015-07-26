using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Data.Fakes;
using SmartConfig.Tests.TestConfigs;

namespace SmartConfig.Tests
{
    [TestClass]
    public class SmartConfigManagerTests
    {
        [TestMethod]
        public void Load_ValueTypeFields()
        {
            var testConfig = new[]
            {
                "ABC||BooleanField|true",
                "ABC||CharField|a",
                "ABC||StringField|abc",
                "ABC||Int16Field|123",
                "ABC||Int32Field|123",
                "ABC||Int64Field|123",
                "ABC||SingleField|1.2",
                "ABC||DoubleField|1.2",
                "ABC||DecimalField|1.2",
            }
            .ToConfigElements();

            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    var elements = testConfig.Where(ce => ce.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    return elements;
                }
            };

            SmartConfigManager.Load<ValueTypeFields>(dataSource);

            Assert.AreEqual(true, ValueTypeFields.BooleanField);
            Assert.AreEqual('a', ValueTypeFields.CharField);
            Assert.AreEqual("abc", ValueTypeFields.StringField);
            Assert.AreEqual(123, ValueTypeFields.Int16Field);
            Assert.AreEqual(123, ValueTypeFields.Int32Field);
            Assert.AreEqual(123, ValueTypeFields.Int64Field);
            Assert.AreEqual(1.2f, ValueTypeFields.SingleField);
            Assert.AreEqual(1.2, ValueTypeFields.DoubleField);
            Assert.AreEqual(1.2m, ValueTypeFields.DecimalField);
        }

        public void Load_NullableValueTypeFields()
        {
            var testConfig = new[]
            {
                "ABC||BooleanField|true",
                "ABC||CharField|a",
                "ABC||StringField|abc",
                "ABC||Int16Field|123",
                "ABC||Int32Field|123",
                "ABC||Int64Field|123",
                "ABC||FloatField|1.2",
                "ABC||DoubleField|1.2",
                "ABC||DecimalField|1.2",
            }.ToConfigElements();

            // Create stub data source:
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    var configElement = testConfig.SingleOrDefault(ce => ce.Name == name);
                    if (configElement == null)
                    {
                        Assert.IsTrue(name.StartsWith("Nullable"));
                        return Enumerable.Empty<ConfigElement>();
                    }
                    var elements = testConfig.Where(ce => ce.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    return elements;
                }
            };

            SmartConfigManager.Load<ValueTypeFields>(dataSource);

            // Check values:

            Assert.AreEqual(true, ValueTypeFields.BooleanField);
            Assert.AreEqual('a', ValueTypeFields.CharField);
            Assert.AreEqual("abc", ValueTypeFields.StringField);
            Assert.AreEqual(123, ValueTypeFields.Int16Field);
            Assert.AreEqual(123, ValueTypeFields.Int32Field);
            Assert.AreEqual(123, ValueTypeFields.Int64Field);
            Assert.AreEqual(1.2f, ValueTypeFields.SingleField);
            Assert.AreEqual(1.2, ValueTypeFields.DoubleField);
            Assert.AreEqual(1.2m, ValueTypeFields.DecimalField);

            //Assert.IsFalse(ValueFields.NullableInt16Field.HasValue);
            //Assert.IsFalse(ValueFields.NullableInt32Field.HasValue);
            //Assert.IsFalse(ValueFields.NullableInt64Field.HasValue);

            //Assert.AreEqual(TestEnum.TestValue2, ValueFields.EnumField);

            //CollectionAssert.AreEqual(new[] { 1, 2, 3 }, ValueFields.ListInt32Field);

            SmartConfigManager.Update(() => ValueTypeFields.StringField, "abcd");
            //SmartConfigManager.Update(() => ValueFields.ListInt32Field, new List<Int32>() { 4, 5, 6 });

            Assert.AreEqual("abcd", ValueTypeFields.StringField);
            //CollectionAssert.AreEqual(new[] { 4, 5, 6 }, ValueFields.ListInt32Field);

        }

        [TestMethod]
        public void TestSingleNestedFields()
        {
            var testConfig = new List<ConfigElement>
            {
                new ConfigElement(){ Name = "SubConfig.StringField", Value = "abc", Environment = "ABC" },
            };

            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    Assert.IsNotNull(testConfig.SingleOrDefault(ce => ce.Name == name));
                    var elements = testConfig.Where(ce => ce.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    return elements;
                }
            };
            SmartConfigManager.Load<SingleNestedFields>(dataSource);

            Assert.AreEqual("abc", SingleNestedFields.SubConfig.StringField);
        }

        [TestMethod]
        public void TestMultipleNestedFields()
        {
            var testConfig = new List<ConfigElement>
            {
                new ConfigElement(){ Name = "SubConfig.SubSubConfig.StringField", Value = "abc", Environment = "ABC" },
            };

            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    Assert.IsNotNull(testConfig.SingleOrDefault(ce => ce.Name == name));
                    var elements = testConfig.Where(ce => ce.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    return elements;
                }
            };
            SmartConfigManager.Load<MultipleNestedFields>(dataSource);

            Assert.AreEqual("abc", MultipleNestedFields.SubConfig.SubSubConfig.StringField);
        }

        [TestMethod]
        public void TestMultiConfigEnabled()
        {
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    Assert.AreEqual("ABCD.StringField", name);
                    return Enumerable.Empty<ConfigElement>();
                }
            };
            SmartConfigManager.Load<ConfigNameTestConfig>(dataSource);
        }

        [TestMethod]
        public void TestVersion()
        {
            var testConfig = new List<ConfigElement>
            {
                new ConfigElement(){ Name = "StringField", Value = "abc", Version = "1.0.0", Environment = "ABC" },
                new ConfigElement(){ Name = "StringField", Value = "def", Version = "2.1.1", Environment = "ABC" },
                new ConfigElement(){ Name = "StringField", Value = "ghi", Version = "3.2.4", Environment = "ABC" },
            };

            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    Assert.IsTrue(testConfig.Any(ce => ce.Name == name));
                    var elements = testConfig.Where(ce => ce.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    return elements;
                }
            };
            SmartConfigManager.Load<VersionTestConfig>(dataSource);

            Assert.AreEqual("def", VersionTestConfig.StringField);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullValueType()
        {
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    Assert.AreEqual("Int32Field", name);
                    return Enumerable.Empty<ConfigElement>();
                }
            };
            SmartConfigManager.Load<NullValueTestConfig>(dataSource);
            Assert.Fail("Exception was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSmartConfigAttributeMissing()
        {
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    Assert.AreEqual("Int32Field", name);
                    return Enumerable.Empty<ConfigElement>();
                }
            };
            SmartConfigManager.Load<MissingAttributeTestConfig>(dataSource);
            Assert.Fail("Exception was not thrown.");
        }

        [TestMethod]
        public void TestSqlServer()
        {
            var dataSource = new SqlServer()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["SmartConfigEntities"].ConnectionString,
                TableName = "TestConfig"
            };
            SmartConfigManager.Load<SqlServerTestConfig>(dataSource);
            Assert.AreEqual(1, SqlServerTestConfig.Int32Field);
        }

        [TestMethod]
        public void TestSqlServerUpdate()
        {
            var dataSource = new SqlServer()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["SmartConfigEntities"].ConnectionString,
                TableName = "TestConfig"
            };
            SmartConfigManager.Load<SqlServerTestConfig>(dataSource);
            Assert.AreEqual(3, SqlServerTestConfig.Int32Field);
            SmartConfigManager.Update(() => SqlServerTestConfig.Int32Field, 4);
        }
    }
}
