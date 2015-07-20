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
using SmartConfig.Fakes;
using SmartConfig.Tests.TestConfigs;

namespace SmartConfig.Tests
{
    [TestClass]
    public class SmartConfigTests
    {
        [TestMethod]
        public void TestRootFields()
        {
            // TODO: Add other data type tests.
            var configElements = new List<ConfigElement>
            {
                // Nullable fields are deactivated:

                new ConfigElement(){ Name = "CharField", Value = "a" },
                new ConfigElement(){ Name = "StringField", Value = "abc" },
                new ConfigElement(){ Name = "Int16Field", Value = "123" },
                //new ConfigElement(){ Name = "Nullable16Field", Value = "123" },
                new ConfigElement(){ Name = "Int32Field", Value = "123" },
                //new ConfigElement(){ Name = "NullableInt32Field", Value = "123" },
                new ConfigElement(){ Name = "Int64Field", Value = "123" },
                //new ConfigElement(){ Name = "NullableInt64Field", Value = "123" },
                
                new ConfigElement(){ Name = "ListInt32Field", Value = "[1, 2, 3]" },
            };

            // Set the same environment and version for all config elements:
            configElements.ForEach(ce => { ce.Environment = "ABC"; ce.Version = string.Empty; });

            // Create stub data source:
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    var configElement = configElements.SingleOrDefault(ce => ce.Name == name);
                    if (configElement == null)
                    {
                        Assert.IsTrue(name.StartsWith("Nullable"));
                        return Enumerable.Empty<ConfigElement>();
                    }
                    var elements = configElements.Where(ce => ce.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    return elements;
                },
                UpdateConfigElement = (configElement) =>
                {
                    var updateConfigElement =
                        configElements
                        .SingleOrDefault(ce =>
                            ce.Environment == configElement.Environment
                            && ce.Version == configElement.Version
                            && ce.Name == configElement.Name);

                    Assert.IsNotNull(updateConfigElement, "ConfigElement to update not found.");
                }
            };

            SmartConfig.Initialize<RootFields>(dataSource);

            // Check values:

            Assert.AreEqual('a', RootFields.CharField);
            Assert.AreEqual("abc", RootFields.StringField);
            Assert.AreEqual(123, RootFields.Int16Field);
            Assert.AreEqual(123, RootFields.Int32Field);
            Assert.AreEqual(123, RootFields.Int64Field);

            Assert.IsFalse(RootFields.NullableInt16Field.HasValue);
            Assert.IsFalse(RootFields.NullableInt32Field.HasValue);
            Assert.IsFalse(RootFields.NullableInt64Field.HasValue);

            CollectionAssert.AreEqual(new Int32[] { 1, 2, 3 }, RootFields.ListInt32Field);

            SmartConfig.Update(() => RootFields.StringField, "abcd");
            SmartConfig.Update(() => RootFields.ListInt32Field, new List<Int32>() { 4, 5, 6 });

            Assert.AreEqual("abcd", RootFields.StringField);
            CollectionAssert.AreEqual(new Int32[] { 4, 5, 6 }, RootFields.ListInt32Field);

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
            SmartConfig.Initialize<SingleNestedFields>(dataSource);

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
            SmartConfig.Initialize<MultipleNestedFields>(dataSource);

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
            SmartConfig.Initialize<ConfigNameTestConfig>(dataSource);
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
            SmartConfig.Initialize<VersionTestConfig>(dataSource);

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
            SmartConfig.Initialize<NullValueTestConfig>(dataSource);
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
            SmartConfig.Initialize<MissingAttributeTestConfig>(dataSource);
            Assert.Fail("Exception was not thrown.");
        }

        [TestMethod]
        public void TestSqlServer()
        {
            var dataSource = new SqlServer()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["SmartConfigEntities"].ConnectionString,
                ConfigTableName = "TestConfig"
            };
            SmartConfig.Initialize<SqlServerTestConfig>(dataSource);
            Assert.AreEqual(1, SqlServerTestConfig.Int32Field);
        }

        [TestMethod]
        public void TestSqlServerUpdate()
        {
            var dataSource = new SqlServer()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["SmartConfigEntities"].ConnectionString,
                ConfigTableName = "TestConfig"
            };
            SmartConfig.Initialize<SqlServerTestConfig>(dataSource);
            Assert.AreEqual(1, SqlServerTestConfig.Int32Field);
            SmartConfig.Update(() => SqlServerTestConfig.Int32Field, 2);
        }
    }
}
