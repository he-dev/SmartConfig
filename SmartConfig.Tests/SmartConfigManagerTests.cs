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
        #region _Basics

        [TestMethod]
        public void Load_ValueTypeFields()
        {
            var testData = new[]
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
                    var element = testData.SingleOrDefault(ce => ce.Name.Equals(keys[CommonKeys.Name], StringComparison.OrdinalIgnoreCase));
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

        [TestMethod]
        public void Load_EnumField()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => "TestValue2"
            };
            SmartConfigManager.Load(typeof(EnumField), dataSource);
            Assert.AreEqual(TestEnum.TestValue2, EnumField.EnumField1);
        }

        [TestMethod]
        public void Load_StringField()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => "abcd"
            };
            SmartConfigManager.Load(typeof(StringField), dataSource);
            Assert.AreEqual("abcd", StringField.StringField);
        }

        [TestMethod]
        public void Load_ConfigName()
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

        [TestMethod]
        public void Load_CustomKey()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) =>
                {
                    Assert.AreEqual("2.2.1", keys[CommonKeys.Version], "Invalid version.");
                    return null;
                }
            };
            SmartConfigManager.Load(typeof(CustomKey), dataSource);
        }

        [TestMethod]
        public void Load_DateTimeFields()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Load_ColorFields()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Load_XmlFields()
        {
            Assert.Fail();
        }

        #endregion

        #region Nestings

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

        #region _CustomConverters

        [TestMethod]
        public void Load_JsonField()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => "[1, 2, 3]"
            };
            SmartConfigManager.Load(typeof(JsonField), dataSource);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, JsonField.ListInt32Field);
        }

        #endregion

        #region _Constraints

        [TestMethod]
        public void Load_OptionalField()
        {
            #region Case 1 - optional field without value

            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => null
            };
            SmartConfigManager.Load(typeof(OptionalField), dataSource);
            Assert.AreEqual("xyz", OptionalField.OptionalStringField, "Invalid defualt value.");

            #endregion

            #region Case 2 - optional field with value

            dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => "abcd"
            };
            SmartConfigManager.Load(typeof(OptionalField), dataSource);
            Assert.AreEqual("abcd", OptionalField.OptionalStringField, "Invalid config value.");

            #endregion
        }

        [TestMethod]
        public void Load_RangeFields()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Load_RegularExpressionField()
        {
            Assert.Fail();
        }

        #endregion

        #region Exception tests       

        [TestMethod]
        [ExpectedException(typeof(OptionalException))]
        public void Load_MissingOptionalAttributeConfig()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => null
            };
            SmartConfigManager.Load(typeof(MissingOptionalAttribute), dataSource);
            Assert.Fail("OptionalException was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(SmartConfigTypeNotFoundException))]
        public void Load_MissingSmartConfigAttribute()
        {
            var dataSource = new TestDataSource()
            {
                SelectFunc = (keys) => null
            };
            SmartConfigManager.Load(typeof(MissingSmartConfigAttribute), dataSource);
            Assert.Fail("SmartConfigTypeNotFoundException was not thrown.");
        }

        #endregion        
    }
}
