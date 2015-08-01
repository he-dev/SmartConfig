using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
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
                "BooleanField|true",
                "CharField|a",
                "Int16Field|123",
                "Int32Field|123",
                "Int64Field|123",
                "SingleField|1.2",
                "DoubleField|1.2",
                "DecimalField|1.2",
            }
            .Select(x => new TestConfigElement2(x));

            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) =>
                {
                    var element = testConfig.SingleOrDefault(ce => ce.Name.Equals(keys[CommonKeys.Name], StringComparison.OrdinalIgnoreCase));
                    return element == null ? null : element.Value;
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

        //[TestMethod]
        //public void Load_NullableValueTypeFields()
        //{
        //    var dataSource = new TestDataSource()
        //    {
        //        SelectFunc = (keys) => Enumerable.Repeat(new TestConfigElement2(""), 1)
        //    };

        //    SmartConfigManager.Load(typeof(NullableValueTypeFields), dataSource);

        //    // Assert fields do not have values:

        //    Assert.IsFalse(NullableValueTypeFields.NullableBooleanField.HasValue);
        //    Assert.IsFalse(NullableValueTypeFields.NullableCharField.HasValue);
        //    Assert.IsFalse(NullableValueTypeFields.NullableInt16Field.HasValue);
        //    Assert.IsFalse(NullableValueTypeFields.NullableInt32Field.HasValue);
        //    Assert.IsFalse(NullableValueTypeFields.NullableInt64Field.HasValue);
        //    Assert.IsFalse(NullableValueTypeFields.NullableSingleField.HasValue);
        //    Assert.IsFalse(NullableValueTypeFields.NullableDoubleField.HasValue);
        //    Assert.IsFalse(NullableValueTypeFields.NullableDecimalField.HasValue);


        //    var testConfig = new[]
        //    {
        //        "ABC||NullableBooleanField|true",
        //        "ABC||NullableCharField|a",
        //        "ABC||NullableInt16Field|123",
        //        "ABC||NullableInt32Field|123",
        //        "ABC||NullableInt64Field|123",
        //        "ABC||NullableSingleField|1.2",
        //        "ABC||NullableDoubleField|1.2",
        //        "ABC||NullableDecimalField|1.2",
        //    }
        //    .ToConfigElements();

        //    dataSource = new StubDataSourceBase()
        //    {
        //        SelectStringStringString = (environment, version, name) => testConfig.Where(ce => ce.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        //    };

        //    SmartConfigManager.Load(typeof(NullableValueTypeFields), dataSource);

        //    Assert.IsTrue(NullableValueTypeFields.NullableBooleanField.HasValue);
        //    Assert.IsTrue(NullableValueTypeFields.NullableCharField.HasValue);
        //    Assert.IsTrue(NullableValueTypeFields.NullableInt16Field.HasValue);
        //    Assert.IsTrue(NullableValueTypeFields.NullableInt32Field.HasValue);
        //    Assert.IsTrue(NullableValueTypeFields.NullableInt64Field.HasValue);
        //    Assert.IsTrue(NullableValueTypeFields.NullableSingleField.HasValue);
        //    Assert.IsTrue(NullableValueTypeFields.NullableDoubleField.HasValue);
        //    Assert.IsTrue(NullableValueTypeFields.NullableDecimalField.HasValue);

        //    Assert.AreEqual(true, NullableValueTypeFields.NullableBooleanField.Value);
        //    Assert.AreEqual('a', NullableValueTypeFields.NullableCharField.Value);
        //    Assert.AreEqual(123, NullableValueTypeFields.NullableInt16Field.Value);
        //    Assert.AreEqual(123, NullableValueTypeFields.NullableInt32Field.Value);
        //    Assert.AreEqual(123, NullableValueTypeFields.NullableInt64Field.Value);
        //    Assert.AreEqual(1.2f, NullableValueTypeFields.NullableSingleField.Value);
        //    Assert.AreEqual(1.2, NullableValueTypeFields.NullableDoubleField.Value);
        //    Assert.AreEqual(1.2m, NullableValueTypeFields.NullableDecimalField.Value);
        //}

        [TestMethod]
        public void Load_EnumFields()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => "TestValue2"
            };

            SmartConfigManager.Load(typeof(EnumFields), dataSource);

            Assert.AreEqual(TestEnum.TestValue2, EnumFields.EnumField1);
        }

        [TestMethod]
        public void Load_StringFields()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => "abcd"
            };
            SmartConfigManager.Load(typeof(StringFields), dataSource);
            Assert.AreEqual("abcd", StringFields.StringField);
        }

        #endregion

        #region Custom converter tests

        [TestMethod]
        public void Load_JsonField()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => "[1, 2, 3]"
            };
            SmartConfigManager.Load(typeof(JsonFields), dataSource);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, JsonFields.ListInt32Field);
        }

        #endregion

        #region Constraint tests

        //[TestMethod]
        //public void Load_NullableFields()
        //{
        //    #region Case-1 - nullable field without value

        //    var dataSource = new TestDataSource()
        //    {
        //        SelectFunc = (keys) => Enumerable.Repeat(new ConfigElement()
        //        {
        //            Environment = environment,
        //            Version = version,
        //            Name = name,
        //            Value = null
        //        }, 1)
        //    };

        //    SmartConfigManager.Load(typeof(NullableFields), dataSource);

        //    Assert.IsNull(NullableFields.NullableStringField);

        //    #endregion

        //    #region Case-2 - nullable field with value

        //    dataSource = new TestDataSource()
        //    {
        //        SelectFunc = (keys) => Enumerable.Repeat(new ConfigElement()
        //        {
        //            Environment = environment,
        //            Version = version,
        //            Name = name,
        //            Value = "abcd"
        //        }, 1)
        //    };

        //    SmartConfigManager.Load(typeof(NullableFields), dataSource);

        //    Assert.IsNotNull(NullableFields.NullableStringField);
        //    Assert.AreEqual("abcd", NullableFields.NullableStringField);

        //    #endregion
        //}

        [TestMethod]
        public void Load_OptionalFields()
        {
            #region Case 1 - optional field without value

            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => null
            };
            SmartConfigManager.Load(typeof(OptionalFields), dataSource);
            Assert.AreEqual("xyz", OptionalFields.OptionalStringField, "Invalid defualt value.");

            #endregion

            #region Case 2 - optional field with value

            dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => "abcd"
            };
            SmartConfigManager.Load(typeof(OptionalFields), dataSource);
            Assert.AreEqual("abcd", OptionalFields.OptionalStringField, "Invalid config value.");

            #endregion
        }

        #endregion

        #region Nesting tests

        [TestMethod]
        public void Load_OneSubClass()
        {
            var testConfig = new[]
            {
                "StringField|abc",
                "SubClass.StringField|xyz",
            }
            .Select(x => new TestConfigElement2(x));

            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) =>
                {
                    var element = testConfig.SingleOrDefault(ce => ce.Name.Equals(keys[CommonKeys.Name], StringComparison.OrdinalIgnoreCase));
                    return element == null ? null : element.Value;
                }
            };
            SmartConfigManager.Load(typeof(OneNestedClass), dataSource);

            Assert.AreEqual("abc", OneNestedClass.StringField);
            Assert.AreEqual("xyz", OneNestedClass.SubClass.StringField);
        }

        [TestMethod]
        public void Load_TwoSubClasses()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) =>
                {
                    Assert.AreEqual("SubClass.SubSubClass.SubSubStringField", keys[CommonKeys.Name], "Invalid element name.");
                    return "abc";
                }
            };
            SmartConfigManager.Load(typeof(TwoNestedClasses), dataSource);
            Assert.AreEqual("abc", TwoNestedClasses.SubClass.SubSubClass.SubSubStringField);
        }

        #endregion

        #region Config name tests

        [TestMethod]
        public void Load_TestMultiConfigEnabled()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) =>
                {
                    Assert.AreEqual("ABCD.StringField", keys[CommonKeys.Name], "Invalid element name.");
                    return null;
                }
            };
            SmartConfigManager.Load(typeof(ConfigName), dataSource);
        }

        #endregion

        #region Version tests

        [TestMethod]
        public void TestVersion()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) =>
                {
                    Assert.AreEqual("2.2.1", keys[CommonKeys.Version], "Invalid version.");
                    return null;
                }
            };
            SmartConfigManager.Load(typeof(VersionTestConfig), dataSource);

        }

        #endregion

        #region Exception tests

        //[TestMethod]
        //[ExpectedException(typeof(NullableException))]
        //public void Load_MissingNullableAttributeConfig()
        //{
        //    var dataSource = new TestDataSource()
        //    {
        //        SelectFunc = (keys) => null
        //    };
        //    SmartConfigManager.Load(typeof(MissingNullableAttributeConfig), dataSource);
        //    Assert.Fail("Exception was not thrown.");
        //}

        [TestMethod]
        [ExpectedException(typeof(OptionalException))]
        public void Load_MissingOptionalAttributeConfig()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => null
            };
            SmartConfigManager.Load(typeof(MissingOptionalAttributeConfig), dataSource);
            Assert.Fail("OptionalException was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(SmartConfigTypeNotFoundException))]
        public void TestSmartConfigAttributeMissing()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => null
            };
            SmartConfigManager.Load(typeof(MissingAttributeTestConfig), dataSource);
            Assert.Fail("SmartConfigTypeNotFoundException was not thrown.");
        }

        #endregion        
    }
}
