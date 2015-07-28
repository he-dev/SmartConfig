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
        #region Basic tests (simple fields)

        [TestMethod]
        public void Load_ValueTypeFields()
        {
            var testConfig = new[]
            {
                "ABC||BooleanField|true",
                "ABC||CharField|a",
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

            SmartConfigManager.Load(typeof(ValueTypeFields), dataSource);

            Assert.AreEqual(true, ValueTypeFields.BooleanField);
            Assert.AreEqual('a', ValueTypeFields.CharField);
            Assert.AreEqual(123, ValueTypeFields.Int16Field);
            Assert.AreEqual(123, ValueTypeFields.Int32Field);
            Assert.AreEqual(123, ValueTypeFields.Int64Field);
            Assert.AreEqual(1.2f, ValueTypeFields.SingleField);
            Assert.AreEqual(1.2, ValueTypeFields.DoubleField);
            Assert.AreEqual(1.2m, ValueTypeFields.DecimalField);
        }

        [TestMethod]
        public void Load_NullableValueTypeFields()
        {
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => Enumerable.Repeat(new ConfigElement() { Environment = "ABC", Name = name, Value = null }, 1)
            };

            SmartConfigManager.Load(typeof(NullableValueTypeFields), dataSource);

            // Assert fields do not have values:

            Assert.IsFalse(NullableValueTypeFields.NullableBooleanField.HasValue);
            Assert.IsFalse(NullableValueTypeFields.NullableCharField.HasValue);
            Assert.IsFalse(NullableValueTypeFields.NullableInt16Field.HasValue);
            Assert.IsFalse(NullableValueTypeFields.NullableInt32Field.HasValue);
            Assert.IsFalse(NullableValueTypeFields.NullableInt64Field.HasValue);
            Assert.IsFalse(NullableValueTypeFields.NullableSingleField.HasValue);
            Assert.IsFalse(NullableValueTypeFields.NullableDoubleField.HasValue);
            Assert.IsFalse(NullableValueTypeFields.NullableDecimalField.HasValue);


            var testConfig = new[]
            {
                "ABC||NullableBooleanField|true",
                "ABC||NullableCharField|a",
                "ABC||NullableInt16Field|123",
                "ABC||NullableInt32Field|123",
                "ABC||NullableInt64Field|123",
                "ABC||NullableSingleField|1.2",
                "ABC||NullableDoubleField|1.2",
                "ABC||NullableDecimalField|1.2",
            }
            .ToConfigElements();

            dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => testConfig.Where(ce => ce.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            };

            SmartConfigManager.Load(typeof(NullableValueTypeFields), dataSource);

            Assert.IsTrue(NullableValueTypeFields.NullableBooleanField.HasValue);
            Assert.IsTrue(NullableValueTypeFields.NullableCharField.HasValue);
            Assert.IsTrue(NullableValueTypeFields.NullableInt16Field.HasValue);
            Assert.IsTrue(NullableValueTypeFields.NullableInt32Field.HasValue);
            Assert.IsTrue(NullableValueTypeFields.NullableInt64Field.HasValue);
            Assert.IsTrue(NullableValueTypeFields.NullableSingleField.HasValue);
            Assert.IsTrue(NullableValueTypeFields.NullableDoubleField.HasValue);
            Assert.IsTrue(NullableValueTypeFields.NullableDecimalField.HasValue);

            Assert.AreEqual(true, NullableValueTypeFields.NullableBooleanField.Value);
            Assert.AreEqual('a', NullableValueTypeFields.NullableCharField.Value);
            Assert.AreEqual(123, NullableValueTypeFields.NullableInt16Field.Value);
            Assert.AreEqual(123, NullableValueTypeFields.NullableInt32Field.Value);
            Assert.AreEqual(123, NullableValueTypeFields.NullableInt64Field.Value);
            Assert.AreEqual(1.2f, NullableValueTypeFields.NullableSingleField.Value);
            Assert.AreEqual(1.2, NullableValueTypeFields.NullableDoubleField.Value);
            Assert.AreEqual(1.2m, NullableValueTypeFields.NullableDecimalField.Value);
        }

        [TestMethod]
        public void Load_EnumFields()
        {
            #region Case-1 - normal field

            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => new[] { "ABC||EnumField1|TestValue2" }.ToConfigElements()
            };

            SmartConfigManager.Load(typeof(EnumFields), dataSource);

            Assert.AreEqual(TestEnum.TestValue2, EnumFields.EnumField1);

            #endregion

            #region Case-2 - nullable field without value

            //dataSource = new StubDataSourceBase()
            //{
            //    SelectString = (name) => Enumerable.Empty<ConfigElement>()
            //};

            //SmartConfigManager.Load<EnumFields>(dataSource);

            //Assert.IsFalse(EnumFields.EnumField2.HasValue);

            #endregion

            #region Case-3 - nullable field with value

            //dataSource = new StubDataSourceBase()
            //{
            //    SelectString = (name) => new[] { "ABC||EnumField2|TestValue3" }.ToConfigElements()
            //};

            //SmartConfigManager.Load<EnumFields>(dataSource);

            //Assert.IsTrue(EnumFields.EnumField2.HasValue);
            //Assert.AreEqual(TestEnum.TestValue3, EnumFields.EnumField2.Value);

            #endregion

        }

        [TestMethod]
        public void Load_StringFields()
        {
            #region Case-1 - normal field

            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => new[] { "ABC||StringField|abcd" }.ToConfigElements()
            };

            SmartConfigManager.Load(typeof(StringFields), dataSource);

            Assert.AreEqual("abcd", StringFields.StringField);

            #endregion
        }

        #endregion

        #region Custom converter tests

        [TestMethod]
        public void Load_JsonField()
        {
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => new[] { "ABC||ListInt32Field|[1, 2, 3]" }.ToConfigElements()
            };

            SmartConfigManager.Load(typeof(JsonFields), dataSource);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, JsonFields.ListInt32Field);
        }

        #endregion

        #region Constraint tests

        [TestMethod]
        public void Load_NullableFields()
        {
            #region Case-1 - nullable field without value

            var dataSource = new StubDataSourceBase()
            {
                // Return null value for all names.
                SelectString = (name) => Enumerable.Repeat(new ConfigElement() { Environment = "ABC", Name = name, Value = null }, 1)
            };

            SmartConfigManager.Load(typeof(NullableFields), dataSource);

            Assert.IsNull(NullableFields.NullableStringField);

            #endregion

            #region Case-2 - nullable field with value

            dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => new[] { "ABC||NullableStringField|abcd" }.ToConfigElements()
            };

            SmartConfigManager.Load(typeof(NullableFields), dataSource);

            Assert.IsNotNull(NullableFields.NullableStringField);
            Assert.AreEqual("abcd", NullableFields.NullableStringField);

            #endregion
        }

        [TestMethod]
        public void Load_OptionalFields()
        {
            #region Case 1 - optional field without value

            var dataSource = new StubDataSourceBase()
            {
                // Return null value for all names.
                SelectString = (name) => Enumerable.Empty<ConfigElement>()
            };

            SmartConfigManager.Load(typeof(OptionalFields), dataSource);

            Assert.AreEqual("xyz", OptionalFields.OptionalStringField);

            #endregion

            #region Case 2 - optional field with value

            dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => new[] { "ABC||OptionalStringField|abcd" }.ToConfigElements()
            };

            SmartConfigManager.Load(typeof(OptionalFields), dataSource);

            Assert.IsNotNull(OptionalFields.OptionalStringField);
            Assert.AreEqual("abcd", OptionalFields.OptionalStringField);

            #endregion
        }

        #endregion

        #region Nesting tests

        [TestMethod]
        public void Load_OneSubClass()
        {
            var testConfig = new[]
            {
                "ABC||StringField|abc",
                "ABC||SubClass.StringField|xyz",
            }
            .ToConfigElements();

            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => testConfig.Where(ce => ce.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            };
            SmartConfigManager.Load(typeof(OneNestedClass), dataSource);

            Assert.AreEqual("abc", OneNestedClass.StringField);
            Assert.AreEqual("xyz", OneNestedClass.SubClass.StringField);
        }

        [TestMethod]
        public void Load_TwoSubClasses()
        {
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => new[] { "ABC||SubClass.SubSubClass.StringField|abc" }.ToConfigElements()
            };
            SmartConfigManager.Load(typeof(TwoNestedClasses), dataSource);

            Assert.AreEqual("abc", TwoNestedClasses.SubClass.SubSubClass.StringField);
        }

        #endregion

        #region Config name tests

        [TestMethod]
        public void Load_TestMultiConfigEnabled()
        {
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) =>
                {
                    Assert.AreEqual("ABCD.StringField", name);
                    return Enumerable.Empty<ConfigElement>();
                }
            };
            SmartConfigManager.Load(typeof(ConfigName), dataSource);
        }

        #endregion

        #region Version tests

        [TestMethod]
        public void TestVersion()
        {
            var testConfig = new[]
            {
                "ABC|1.0.0|StringField|abc",
                "ABC|2.1.1|StringField|jkl",
                "ABC|3.2.4|StringField|xyz",
            }
            .ToConfigElements();

            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => testConfig
            };
            SmartConfigManager.Load(typeof(VersionTestConfig), dataSource);

            Assert.AreEqual("jkl", VersionTestConfig.StringField);
        }

        #endregion

        #region Database tests

        [TestMethod]
        public void TestSqlServer()
        {
            var dataSource = new SqlServer()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                TableName = "TestConfig"
            };
            SmartConfigManager.Load(typeof(SqlServerTestConfig), dataSource);
            Assert.AreEqual(1, SqlServerTestConfig.Int32Field);
        }

        [TestMethod]
        public void TestSqlServerUpdate()
        {
            var dataSource = new SqlServer()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                TableName = "TestConfig"
            };
            SmartConfigManager.Load(typeof(SqlServerTestConfig), dataSource);
            Assert.AreEqual(3, SqlServerTestConfig.Int32Field);
            SmartConfigManager.Update(() => SqlServerTestConfig.Int32Field, 4);
        }

        #endregion

        #region Exception tests

        [TestMethod]
        [ExpectedException(typeof(ConfigElementNotFounException))]
        public void Load_NullReferenceTypeFields()
        {
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => Enumerable.Empty<ConfigElement>()
            };
            SmartConfigManager.Load(typeof(NullReferenceTypeFields), dataSource);
            Assert.Fail("Exception was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigElementNotFounException))]
        public void Load_NullValueTypeFields()
        {
            var dataSource = new StubDataSourceBase()
            {
                SelectString = (name) => Enumerable.Empty<ConfigElement>()
            };
            SmartConfigManager.Load(typeof(NullValueTypeFields), dataSource);
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
            SmartConfigManager.Load(typeof(MissingAttributeTestConfig), dataSource);
            Assert.Fail("Exception was not thrown.");
        }

        #endregion
    }
}
